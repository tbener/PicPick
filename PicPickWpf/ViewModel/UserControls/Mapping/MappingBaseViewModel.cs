using PicPick.Models;
using PicPick.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingBaseViewModel
    {
        public MappingBaseViewModel(IActivity activity)
        {
            // Source Pane
            PicPickProjectActivitySource source = activity.Source;
            var subFolders = source.IncludeSubFolders ? "including sub-folders" : "not including sub-folders";
            SourceDisplay = $"{source.Path} ({source.Filter}), {subFolders}";
            SourceFoundFiles = $"{activity.FilesGraph.Files.Count} files found";

            // Destinations Pane
            var lookup = activity.FilesGraph.DestinationFolders.Values.ToLookup(df => df.BasedOnDestination);
            DestinationList = lookup.Select(item => new MappingDestinationViewModel(item.Key, item.ToList(), activity)).ToList();
        }

        public List<MappingDestinationViewModel> DestinationList { get; set; }

        public string SourceDisplay { get; set; }
        public string SourceFoundFiles { get; set; }
    }
}
