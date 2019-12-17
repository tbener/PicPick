using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingDestinationViewModel
    {
        public MappingDestinationViewModel(PicPickProjectActivityDestination baseDestination, List<DestinationFolder> destinationFolders)
        {
            DestinationDisplay = $"{baseDestination.Path}\\{{{baseDestination.Template}}}";
            FolderList = destinationFolders.Select(df => new MappingFolderViewModel(df)).ToList();
        }

        public string DestinationDisplay { get; set; }

        public List<MappingFolderViewModel> FolderList { get; set; }
    }
}
