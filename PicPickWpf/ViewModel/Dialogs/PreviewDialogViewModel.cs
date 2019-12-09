using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicPick.Models;
using PicPick.ViewModel.UserControls;

namespace PicPick.ViewModel.Dialogs
{
    public class PreviewDialogViewModel
    {
        public PreviewDialogViewModel(ActivityFileMapping fileMapping)
        {
            MappingViewModel = new MappingPlanViewModel(fileMapping);
        }

        public MappingPlanViewModel MappingViewModel { get; set; }
    }
}
