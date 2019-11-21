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




        internal void Clear()
        {
            SourceFiles.Clear();
            DestinationFolders.Clear();
        }

        internal void AddFile(string file, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        internal void AddFile(string file, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Compute(PicPickProjectActivitySource source, List<PicPickProjectActivityDestination> destinations)
        {
            ImageFileInfo fileDateInfo = new ImageFileInfo();

            Clear();

            bool needDates = destinations.Any(d => d.HasTemplate);

            // add the source files
            foreach (string file in source.FileList)
            {
                SourceFiles.Add(file, new SourceFile(file, needDates));

                //await Task.Run(() => progressInfo.Advance());
                //cancellationToken.ThrowIfCancellationRequested();
            }

            foreach (PicPickProjectActivityDestination destination in destinations)
            {
                List<SourceFile> sourceFiles = SourceFiles.Values.ToList();

                if (destination.HasTemplate)
                {
                    foreach (SourceFile sourceFile in sourceFiles)
                    {
                        string relPath = destination.GetTemplatePath(sourceFile.DateTime);
                        string fullPath = PathHelper.GetFullPath(destination.PathAbsolute, relPath);

                        if (!DestinationFolders.TryGetValue(fullPath, out DestinationFolder destinationFolder))
                        {
                            destinationFolder = new DestinationFolder(fullPath, destination);
                            DestinationFolders.Add(fullPath, destinationFolder);
                        }
                        // This will do both adding a reference from the SourceFile to the destinationFolder and adding a new DestinationFile object to this destinationFolder
                        destinationFolder.AddFile(sourceFile);
                    }
                }
                else
                {
                    // it will be a single DestinationFolder for all files

                    foreach (SourceFile sourceFile in sourceFiles)
                    {

                    }

                }
            }
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
                ImageFileInfo.TryGetDateTaken_(fullPath, out DateTime dateTime);
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
        private PicPickProjectActivityDestination destination;

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
            Files.Add(new DestinationFile(sourceFile.FileName));
        }

        public string FullPath { get; set; }
        public bool IsNew { get; set; }
        public bool Created { get; set; }
        public List<DestinationFile> Files { get; set; }
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
        public DestinationFile(string fileName)
        {
            OriginalFileName = fileName;
        }

        public string OriginalFileName { get; set; }
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
