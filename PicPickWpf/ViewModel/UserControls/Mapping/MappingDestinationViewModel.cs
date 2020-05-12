using PicPick.Models;
using PicPick.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingDestinationViewModel
    {
        public MappingDestinationViewModel(PicPickProjectActivityDestination baseDestination, List<DestinationFolder> destinationFolders, PicPickProjectActivity activity)
        {
            DestinationDisplay = $"{baseDestination.Path}\\{{{baseDestination.Template}}}";
            if (activity.State == ACTIVITY_STATE.DONE)
                FolderList = destinationFolders.Select(df => new MappingResultsFolderViewModel(df) as MappingFolderViewModel).ToList();
            else
                FolderList = destinationFolders.Select(df => new MappingPlanFolderViewModel(df) as MappingFolderViewModel).ToList();
        }

        public string DestinationDisplay { get; set; }

        public List<MappingFolderViewModel> FolderList { get; set; }
    }
}
