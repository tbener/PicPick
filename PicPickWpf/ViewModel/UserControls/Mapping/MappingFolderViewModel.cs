using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingFolderViewModel
    {
        public MappingFolderViewModel(DestinationFolder destinationFolder)
        {
            Folder = PathHelper.GetRelativePath(destinationFolder.BasedOnDestination.Path, destinationFolder.FullPath);
            FilesCount = $"{destinationFolder.Files.Count} files";
            State = destinationFolder.IsNew ? "New" : "Exists";
        }

        public string Folder { get; set; }
        public string FilesCount { get; set; }
        public string State { get; set; }
    }
}
