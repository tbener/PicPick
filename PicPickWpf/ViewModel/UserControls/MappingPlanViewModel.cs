using PicPick.Models;
using PicPick.ViewModel.UserControls.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class MappingPlanViewModel
    {
        public MappingPlanViewModel(PicPickProjectActivity activity)
        {
            // Source Pane
            PicPickProjectActivitySource source = activity.Source;
            string subFolders = source.IncludeSubFolders ? "including sub-folders" : "not including sub-folders";
            SourceDisplay = $"{source.Path} ({source.Filter}), {subFolders}";
            SourceFoundFiles = $"{source.FileList.Count} files found";
            SourceFilesDeleteWarning = activity.DeleteSourceFiles ? "The files will be deleted!" : "The files won't be deleted.";

            // Destinations Pane
            var lookup = activity.FileMapping.DestinationFolders.Values.ToLookup(df => df.BasedOnDestination);
            DestinationList = lookup.Select(item => new MappingDestinationViewModel(item.Key, item.ToList())).ToList();
        }

        public List<MappingDestinationViewModel> DestinationList { get; set; }

        public string SourceDisplay { get; set; }
        public string SourceFoundFiles { get; set; }
        public string SourceFilesDeleteWarning { get; set; }
    }
}
