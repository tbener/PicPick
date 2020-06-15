using log4net;
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

namespace PicPick.Models.Mapping
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

        private Dictionary<string, SourceFile> _sourceFiles { get; set; } = new Dictionary<string, SourceFile>();
        private Dictionary<string, SourceFile> _filteredSourceFiles { get; set; } = new Dictionary<string, SourceFile>();

        public Dictionary<string, DestinationFolder> DestinationFolders { get; set; } = new Dictionary<string, DestinationFolder>();
        public List<PicPickProjectActivityDestination> Destinations;
        public IActivity Activity { get; set; }

        public ActivityFileMapping(IActivity activity)
        {
            Activity = activity;
        }


        internal void Clear()
        {
            _sourceFiles.Clear();
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
            progressInfo.Text = "Analysing...";
            progressInfo.Report();
            Clear();
            ValidateFields();

            Destinations = destinations;

            bool needDates = destinations.Any(d => d.HasTemplate);
            needDates = needDates || Activity.Source.FromDate.Use || Activity.Source.ToDate.Use;

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
                foreach (var sourceFile in fileList)
                {
                    sourceFiles.Add(new SourceFile(sourceFile, needDates));
                    progressInfo.Advance();
                    progressInfo.CancellationToken.ThrowIfCancellationRequested();
                }
            });

            // ####

            // Add Source Files to dictionary
            if (Activity.Source.FromDate.Use || Activity.Source.ToDate.Use)
            {
                DateTime fromDate = Activity.Source.FromDate.Use ? Activity.Source.FromDate.Date : DateTime.MinValue;
                DateTime toDate = Activity.Source.ToDate.Use ? Activity.Source.ToDate.Date : DateTime.MaxValue;
                foreach (SourceFile sf in sourceFiles)
                {
                    if (sf.DateTime >= fromDate && sf.DateTime <= toDate)
                        _sourceFiles.Add(sf.FullFileName, sf);
                }
            }
            else
                sourceFiles.ForEach(sf => _sourceFiles.Add(sf.FullFileName, sf));

            List<SourceFile> filesToRemove = new List<SourceFile>();

            // the following loop should be very quick
            // no need for progress update
            foreach (PicPickProjectActivityDestination destination in destinations)
            {
                if (destination.HasTemplate)
                {
                    foreach (SourceFile sourceFile in _sourceFiles.Values)
                    {
                        var destinationFullPath = destination.GetFullPath(sourceFile.DateTime);

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

            ApplyFinalFilters();

            _log.Info("Plan is:\n" + ToString());
        }

        public Dictionary<string, SourceFile> SourceFiles 
        {
            get
            {
                return _filteredSourceFiles;
            }
        }

        public void ApplyFinalFilters()
        {
            if (Activity.Source.OnlyNewFiles)
                _filteredSourceFiles = _sourceFiles.Where(sf => !sf.Value.ExistsInDestination).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                _filteredSourceFiles = _sourceFiles;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("----- Plan Start -----\n");

            sb.AppendLine($"Files count: {_sourceFiles.Count}");
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
}
