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
            PicPickProjectActivitySource source = activity.Source;
            string subFolders = source.IncludeSubFolders ? "including sub-folders" : "not including sub-folders";
            SourceDisplay = $"{source.Path} ({source.Filter}), {subFolders}";
            SourceFoundFiles = $"{source.FileList.Count} files found";
            SourceFilesDeleteWarning = activity.DeleteSourceFiles ? "The files will be deleted!" : "The files won't be deleted.";


            ActivityFileMapping mapping = activity.FileMapping;

            Dictionary<PicPickProjectActivityDestination, List<DestinationFolder>> destinations = new Dictionary<PicPickProjectActivityDestination, List<DestinationFolder>>();

            foreach (var dest in mapping.Destinations)
            {
                destinations.Add(dest, new List<DestinationFolder>());
            }
            foreach (var destinationFolder in mapping.DestinationFolders.Values)
            {
                destinations[destinationFolder.BasedOnDestination].Add(destinationFolder);
            }

            DestinationList = destinations.Select(d => new MappingDestinationViewModel(d.Key, d.Value)).ToList();
        }

        public List<MappingDestinationViewModel> DestinationList { get; set; }

        public string SourceDisplay { get; set; }
        public string SourceFoundFiles { get; set; }
        public string SourceFilesDeleteWarning { get; set; }
    }
}
