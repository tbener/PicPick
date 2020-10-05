using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingBaseViewModel : ActivityBaseViewModel
    {
        public MappingBaseViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            // Source Pane
            PicPickProjectActivitySource source = activity.Source;
            var subFolders = source.IncludeSubFolders ? "including sub-folders" : "not including sub-folders";
            SourceDisplay = $"{source.Path} ({source.Filter}), {subFolders}";
        }

        public List<MappingDestinationViewModel> DestinationList
        {
            get
            {
                var lookup = Activity.FileGraph.DestinationFolders.ToLookup(df => df.BasedOnDestination);
                return lookup.Select(item => new MappingDestinationViewModel(item.Key, item.ToList(), Activity)).ToList();
            }
        }

        public string SourceFoundFiles
        {
            get
            {
                try
                {
                    return $"{Activity.FileGraph.Files.Count} files found";
                }
                catch
                {
                    return "";
                }
            }
        }

        public string SourceDisplay { get; set; }

        
    }
}
