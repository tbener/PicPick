using PicPick.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using TalUtils;

namespace PicPick.Models.Mapping
{
    /// <summary>
    /// Represents a real destination path.
    /// A single source files is expected to have a reference to this class as many as the active destinations.
    /// </summary>
    public class DestinationFolder
    {
        private List<DestinationFile> _allFiles = new List<DestinationFile>();
        private List<DestinationFile> _newFiles = new List<DestinationFile>();
        private IActivity _activity;

        public DestinationFolder(string fullPath, PicPickProjectActivityDestination destination, IActivity activity)
        {
            FullPath = fullPath;
            BasedOnDestination = destination;
            IsNew = !PathHelper.Exists(fullPath);
            Created = false;
            _activity = activity;
        }

        public void AddFile(SourceFile sourceFile)
        {
            sourceFile.DestinationFolders.Add(this);
            var df = new DestinationFile(sourceFile, this);
            _allFiles.Add(df);
            if (!sourceFile.ExistsInDestination)
                _newFiles.Add(df);
            DestinationFiles.Add(sourceFile, df);
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
        public Dictionary<SourceFile, DestinationFile> DestinationFiles { get; } = new Dictionary<SourceFile, DestinationFile>();
        public PicPickProjectActivityDestination BasedOnDestination { get; set; }

        public List<DestinationFile> Files
        {
            get
            {
                return _activity.Source.OnlyNewFiles ? _newFiles : _allFiles;
            }
        }

        public bool HasError()
        {
            return _allFiles.Any(ds => ds.HasError());
        }
    }
}
