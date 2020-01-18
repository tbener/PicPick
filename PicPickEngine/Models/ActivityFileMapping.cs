﻿using log4net;
using PicPick.Core;
using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Models
{
    /// <summary>
    /// This class is a dictionary that 
    /// - its Key is the Source File path
    /// - its Value is a SourceFile class
    /// Every SourceFile class splits into its destinations, and then to the destination files.
    /// 
    /// These classes are updated during the analyzing process and then during the running porcess itself.
    /// </summary>
    public class ActivityFileMapping
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        public Dictionary<string, SourceFile> SourceFiles { get; set; } = new Dictionary<string, SourceFile>();
        public Dictionary<string, DestinationFolder> DestinationFolders { get; set; } = new Dictionary<string, DestinationFolder>();
        public List<PicPickProjectActivityDestination> Destinations;
        public IActivity Activity { get; set; }

        public ActivityFileMapping(IActivity activity)
        {
            Activity = activity;
        }


        internal void Clear()
        {
            SourceFiles.Clear();
            DestinationFolders.Clear();
            Destinations = null;
        }

        public async Task ComputeAsync(ProgressInformation progressInfo)
        {
            var activeDestinations = Activity.DestinationList.Where(d => d.Active).ToList();
            await Activity.FileMapping.ComputeAsync(progressInfo, Activity.Source, activeDestinations);
        }

        public async Task ComputeAsync(ProgressInformation progressInfo, PicPickProjectActivitySource source, List<PicPickProjectActivityDestination> destinations)
        {
            progressInfo.Text = "Analying...";
            progressInfo.Report();
            //await Task.Run(() => progressInfo.Report());
            Clear();
            ValidateFields();

            Destinations = destinations;

            bool needDates = destinations.Any(d => d.HasTemplate);

            //   -- The Short Way!!!
            // Create SourceFile list
            // List<SourceFile> sourceFiles = await Task.Run(() => source.FileList.Select(f => new SourceFile(f, needDates)).ToList());

            //   -- The Long Way...
            // ####
            // (note that the loop is not slowing down (relative to the Linq expression) but the progress update does)
            List<SourceFile> sourceFiles = new List<SourceFile>();
            var fileList = source.FileList;

            progressInfo.Maximum = fileList.Count();
            progressInfo.Value = 0;
            await Task.Run(() =>
            {
                foreach (string sourceFile in fileList)
                {
                    sourceFiles.Add(new SourceFile(sourceFile, needDates));
                    progressInfo.Advance();
                    progressInfo.CancellationToken.ThrowIfCancellationRequested();
                }
            });

            // ####

            // Add to dictionary
            sourceFiles.ForEach(sf => SourceFiles.Add(sf.FullFileName, sf));

            // the following loop should be very quick
            // no need for progress update
            foreach (PicPickProjectActivityDestination destination in destinations)
            {
                if (destination.HasTemplate)
                {
                    foreach (SourceFile sourceFile in sourceFiles)
                    {
                        string destinationFullPath = destination.GetFullPath(sourceFile.DateTime);

                        if (!DestinationFolders.TryGetValue(destinationFullPath, out DestinationFolder destinationFolder))
                        {
                            destinationFolder = new DestinationFolder(destinationFullPath, destination);
                            DestinationFolders.Add(destinationFullPath, destinationFolder);
                        }
                        // This will do both adding a reference from the SourceFile to the destinationFolder and adding a new DestinationFile object to this destinationFolder
                        destinationFolder.AddFile(sourceFile);
                    }
                }
                else
                {
                    // it will be a single DestinationFolder for all files
                    DestinationFolder destinationFolder = new DestinationFolder(destination.Path, destination);
                    DestinationFolders.Add(destinationFolder.FullPath, destinationFolder);
                    sourceFiles.ForEach(destinationFolder.AddFile);
                }
            }


            _log.Info("Plan is:\n" + ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("----- Plan Start -----\n");

            sb.AppendLine($"Files count: {SourceFiles.Count}");
            sb.AppendLine($"Destination folders count: {DestinationFolders.Count}");

            sb.AppendLine("\nActive Destinations:");
            foreach (PicPickProjectActivityDestination dest in Destinations)
            {
                sb.AppendLine($"{dest.Path}\\[{dest.Template}]");
            }

            sb.AppendLine();

            foreach (DestinationFolder destination in DestinationFolders.Values)
            {
                sb.AppendLine(string.Format("{0} ({1})", destination.FullPath, destination.IsNew ? "New" : "Exists"));
                foreach (DestinationFile file in destination.Files)
                {
                    sb.AppendLine($"--> {file.GetFullName()}");
                }
            }

            //foreach (SourceFile sourceFile in SourceFiles.Values)
            //{
            //    sb.AppendLine(sourceFile.FullPath);
            //    foreach (DestinationFolder destinationFolder in sourceFile.DestinationFolders)
            //    {
            //        sb.AppendLine($"--> {destinationFolder.FullPath}");
            //    }
            //}

            sb.AppendLine("----- Plan End -----");

            return sb.ToString();
        }

        public void ValidateFields()
        {
            string realPath;
            bool isRealTemplate;

            if (Activity.Source == null || Activity.Source.Path == "")
                throw new NoSourceException();

            if (!Directory.Exists(Activity.Source.Path))
                throw new SourceDirectoryNotFoundException();

            if (Activity.DestinationList.Count == 0)
                throw new NoDestinationsException();

            foreach (var dest in Activity.DestinationList.Where(d => d.Active))
            {
                realPath = dest.GetTemplatePath(DateTime.Now);
                isRealTemplate = !realPath.Equals(dest.Template);
                if (isRealTemplate)
                    continue;

                realPath = Path.Combine(PathHelper.GetFullPath(Activity.Source.Path, dest.Path), dest.Template);

                if (realPath.Equals(Activity.Source.Path, StringComparison.OrdinalIgnoreCase))
                    throw new DestinationEqualsSourceException();
            }
        }

    }



    public class SourceFile
    {
        public SourceFile(string fullPath, bool needDate)
        {
            FullFileName = fullPath;
            FileName = Path.GetFileName(fullPath);
            if (needDate)
            {
                if (!ImageFileInfo.TryGetFileDate(fullPath, out DateTime dateTime))
                    throw new Exception($"Could not extract date from file: {fullPath}");
                DateTime = dateTime;
            }
        }

        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }
        public List<DestinationFolder> DestinationFolders { get; set; } = new List<DestinationFolder>();
        public FILE_STATUS Status { get; private set; } = FILE_STATUS.NONE;

        public bool HasError()
        {
            return DestinationFolders.Any(dm => dm.HasError());
        }

        /// <summary>
        /// Source File holds a status that accumulates all its destinations.
        /// The order is: None -> Copied -> Skipped -> Error
        /// The highest status wins.
        /// (e.g. if one destination copied and one skipped, the final status is Skipped).
        /// </summary>
        /// <param name="status"></param>
        internal void ReportStatus(FILE_STATUS status)
        {
            if (Status < status)
                Status = status;
        }
    }

    /// <summary>
    /// Represents a real destination path.
    /// A single source files is expected to have a reference to this class as many as the active destinations.
    /// </summary>
    public class DestinationFolder
    {
        public DestinationFolder(string fullPath, PicPickProjectActivityDestination destination)
        {
            FullPath = fullPath;
            BasedOnDestination = destination;
            IsNew = !PathHelper.Exists(fullPath);
            Created = false;
        }

        public void AddFile(SourceFile sourceFile)
        {
            sourceFile.DestinationFolders.Add(this);
            Files.Add(new DestinationFile(sourceFile, this));
        }

        public string RelativePath
        {
            get
            {
                try
                {
                    return PathHelper.GetRelativePath(BasedOnDestination.Path, FullPath);
                }
                catch
                {
                    return "";
                }
            }
        }

        public string FullPath { get; set; }
        public bool IsNew { get; set; }
        public bool Created { get; set; }
        public List<DestinationFile> Files { get; set; } = new List<DestinationFile>();
        public PicPickProjectActivityDestination BasedOnDestination { get; set; }



        public bool HasError()
        {
            return Files.Any(ds => ds.HasError());
        }
    }

    /// <summary>
    /// Represents a file in the destination folder
    /// </summary>
    public class DestinationFile
    {
        private bool? _exists;

        public DestinationFile(SourceFile sourceFile, DestinationFolder destinationFolder)
        {
            SourceFile = sourceFile;
            ParentFolder = destinationFolder;
            // if the folder is new - it will return false, so we don't need to check the file.
            // IMPORTANT: this value might change during the process (as new files might appear). This is why the property is a method with [force] parameter.
            _exists = !(destinationFolder.IsNew || !File.Exists(Path.Combine(destinationFolder.FullPath, sourceFile.FileName)));
        }

        public string GetFullName()
        {
            string fileName = string.IsNullOrEmpty(NewName) ? SourceFile.FileName : NewName;
            return Path.Combine(ParentFolder.FullPath, fileName);
        }

        public void SetStatus(FILE_STATUS status)
        {
            Status = status;
            SourceFile.ReportStatus(status);
        }

        public DestinationFolder ParentFolder { get; set; }
        public SourceFile SourceFile { get; set; }

        /// <summary>
        /// Returns whether the file exists in the destination.
        /// The backing field is initialized in the CTOR but it might be changed during the execution.
        /// For that purpose the forceCheck is actually checking the file.
        /// </summary>
        /// <param name="forceCheck">Force checking if the file actually exists in the destination.</param>
        /// <returns>True if the file exists in the destination.</returns>
        public bool Exists(bool forceCheck = false)
        {
            if (!_exists.HasValue || forceCheck)
                _exists = File.Exists(Path.Combine(ParentFolder.FullPath, SourceFile.FileName));
            return _exists.Value;
        }
        
        public string NewName { get; set; }
        public FILE_STATUS Status { get; private set; }
        public Exception Exception { get; set; }

        public bool HasError()
        {
            return Exception != null;
        }
    }
}
