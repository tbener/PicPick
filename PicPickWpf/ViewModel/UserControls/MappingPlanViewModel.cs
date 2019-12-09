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
        public MappingPlanViewModel(ActivityFileMapping mapping)
        {
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
    }
}
