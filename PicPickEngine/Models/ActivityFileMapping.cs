using PicPick.Core;
using PicPick.Helpers;
using System;
using System.Collections.Generic;
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
        public Dictionary<string, DestinationMapping> DestinationMappings { get; set; } = new Dictionary<string, DestinationMapping>();


        internal void SetFile(string file, DateTime? dateTime)
        {
            SourceFile sourceFile;
            if (SourceFiles.ContainsKey(file))
            {
                sourceFile = SourceFiles[file];
            }
            else
            {
                sourceFile = new SourceFile();
                SourceFiles.Add(file, sourceFile);
            }
            sourceFile.DateTime = dateTime;
        }

        internal void Clear()
        {
            SourceFiles.Clear();
            DestinationMappings.Clear();
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
            DateTime dateTime = null;
            ImageFileInfo fileDateInfo = new ImageFileInfo();

            Clear();

            bool needDates = destinations.Any(d => d.HasTemplate);

            // add the source files
            foreach (string file in source.FileList)
            {
                if (needDates)
                    if (fileDateInfo.GetFileDate(file, out dateTime))
                        AddFile(file, dateTime);
                    else
                        AddFile(file, new Exception($"Error extracting date from file: {file}"));
                else
                    AddFile(file);

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

                        if (!DestinationMappings.ContainsKey(fullPath))
                        {
                            DestinationMappings.Add(fullPath, new DestinationMapping(fullPath, destination));
                        }
                    }
                }
                else
                {

                }
            }
        }

        private void AddFile(string file)
        {
            throw new NotImplementedException();
        }
    }

    public class SourceFile
    {
        public string FullPath { get; set; }
        public DateTime DateTime { get; set; }
        public List<DestinationMapping> DestinationMappings { get; set; } = new List<DestinationMapping>();

        public bool HasError()
        {
            return DestinationMappings.Any(dm => dm.HasError());
        }
    }

    /// <summary>
    /// Represents a real destination path.
    /// A single source files is expected to have a reference to this class as many as the active destinations.
    /// </summary>
    public class DestinationMapping
    {
        private PicPickProjectActivityDestination destination;

        public DestinationMapping(string fullPath, PicPickProjectActivityDestination destination)
        {
            FullPath = fullPath;
            BasedOnDestination = destination;
            IsNew = !PathHelper.Exists(fullPath);

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
        public string OriginalName { get; set; }
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
