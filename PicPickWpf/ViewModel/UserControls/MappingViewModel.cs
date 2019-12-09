using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class MappingViewModel
    {
        public MappingViewModel(ActivityFileMapping mapping)
        {
            Mapping = mapping;

            NewFolders = Mapping.DestinationFolders.Values.Where(df => df.IsNew).ToList();
        }

        public ActivityFileMapping Mapping { get; set; }

        public List<DestinationFolder> NewFolders { get; set; }
    }
}
