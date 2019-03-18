using PicPick.Models;
using PicPick.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.UserControls.ViewModel
{
    public class ProgressViewModel : BaseViewModel
    {
        ProgressInformation _progressInfo;

        public ProgressViewModel(ProgressInformation progressInfo)
        {
            ProgressInfo = progressInfo;
            ((Progress<ProgressInformation>)ProgressInfo.Progress).ProgressChanged += (s, e) => OnPropertyChanged(nameof(ProgressPercentsText));
        }

        public string ProgressPercentsText => $"{ProgressInfo.Value} of {ProgressInfo.Maximum}";

        public ProgressInformation ProgressInfo { get => _progressInfo; set => _progressInfo = value; }
    }
}
