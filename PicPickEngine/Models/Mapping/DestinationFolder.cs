using System.Collections.Generic;
using TalUtils;

namespace PicPick.Models.Mapping
{
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
}
