using PicPick.Core;
using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public Dictionary<string, SourceFile> SourceFiles { get; set; } = new Dictionary<string, SourceFile>();
        public Dictionary<string, DestinationFolder> DestinationFolders { get; set; } = new Dictionary<string, DestinationFolder>();
        public List<PicPickProjectActivityDestination> Destinations;



        internal void Clear()
        {
            SourceFiles.Clear();
            DestinationFolders.Clear();
            Destinations = null;
        }

        public void Compute(PicPickProjectActivitySource source, List<PicPickProjectActivityDestination> destinations)
        {
            Clear();

            Destinations = destinations;

            bool needDates = destinations.Any(d => d.HasTemplate);

            // Create SourceFile list
            List<SourceFile> sourceFiles = source.FileList.Select(f => new SourceFile(f, needDates)).ToList();
            // Add to dictionary
            sourceFiles.ForEach(sf => SourceFiles.Add(sf.FullPath, sf));

            // add the source files
            //foreach (string file in source.FileList)
            //{
            //    SourceFiles.Add(file, new SourceFile(file, needDates));

            //    //await Task.Run(() => progressInfo.Advance());
            //    //cancellationToken.ThrowIfCancellationRequested();
            //}

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
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");

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

            return sb.ToString();
        }

    }

    public class SourceFile
    {
        public SourceFile(string fullPath, bool needDate)
        {
            FullPath = fullPath;
            FileName = Path.GetFileName(fullPath);
            if (needDate)
            {
                if (!ImageFileInfo.TryGetFileDate(fullPath, out DateTime dateTime))
                    throw new Exception($"Could not extract date from file: {fullPath}");
                DateTime = dateTime;
            }
        }

        public string FullPath { get; set; }
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }
        public List<DestinationFolder> DestinationFolders { get; set; } = new List<DestinationFolder>();

        public bool HasError()
        {
            return DestinationFolders.Any(dm => dm.HasError());
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
        public DestinationFile(SourceFile sourceFile, DestinationFolder destinationFolder)
        {
            SourceFile = sourceFile;
            ParentFolder = destinationFolder;
            Exists = !(destinationFolder.IsNew || File.Exists(Path.Combine(destinationFolder.FullPath, sourceFile.FileName)));
        }

        public string GetFullName()
        {
            string fileName = string.IsNullOrEmpty(NewName) ? SourceFile.FileName : NewName;
            return Path.Combine(ParentFolder.FullPath, fileName);
        }

        public DestinationFolder ParentFolder { get; set; }
        public SourceFile SourceFile { get; set; }
        public bool Exists { get; set; }
        public string NewName { get; set; }
        public FILE_STATUS Status { get; set; }
        public Exception Exception { get; set; }

        public bool HasError()
        {
            return Exception != null;
        }
    }
}
