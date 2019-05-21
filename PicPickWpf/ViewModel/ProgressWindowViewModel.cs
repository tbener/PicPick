using PicPick.Helpers;
using PicPick.UserControls.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel
{
    public class ProgressWindowViewModel
    {
        public ProgressWindowViewModel(ProgressInformation progressInfo)
        {
            ProgressViewModel = new ProgressViewModel(progressInfo);
        }

        public ProgressViewModel ProgressViewModel { get; private set; }
    }
}
