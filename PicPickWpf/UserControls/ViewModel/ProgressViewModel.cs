using PicPick.Commands;
using PicPick.Models;
using PicPick.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{
    public class ProgressViewModel : BaseViewModel
    {
        ProgressInformation _progressInfo;

        public ICommand CancelCommand { get; set; }

        public ProgressViewModel(ProgressInformation progressInfo)
        {
            ProgressInfo = progressInfo;
            ((Progress<ProgressInformation>)ProgressInfo.Progress).ProgressChanged += (s, e) => OnPropertyChanged(nameof(ProgressPercentsText));

            CancelCommand = new RelayCommand(Cancel, CanCancel);
        }

        private bool CanCancel(object obj)
        {
            return false;
        }

        private void Cancel()
        {
            throw new NotImplementedException();
        }

        public string ProgressPercentsText => $"{ProgressInfo.Value} of {ProgressInfo.Maximum}";

        public ProgressInformation ProgressInfo { get => _progressInfo; set => _progressInfo = value; }
    }
}
