using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.UserControls.ViewModel
{
    public class ProgressViewModel
    {
        ProgressInformation _progressInfo;

        public ProgressViewModel(ProgressInformation progressInfo)
        {
            ProgressInfo = progressInfo;
        }

        public string ProgressPercentsText => $"{ProgressInfo.Value} of {ProgressInfo.Maximum}";

        public ProgressInformation ProgressInfo { get => _progressInfo; set => _progressInfo = value; }
    }
}
