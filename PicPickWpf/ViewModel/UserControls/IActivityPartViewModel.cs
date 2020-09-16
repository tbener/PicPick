using PicPick.Helpers;
using System;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    public interface IActivityPartViewModel : IDisposable
    {
        bool Enabled { get; }
        bool IsRunning { get; set; }
        ProgressInformation ProgressInfo { get; set; }
        ICommand StopCommand { get; set; }
    }
}