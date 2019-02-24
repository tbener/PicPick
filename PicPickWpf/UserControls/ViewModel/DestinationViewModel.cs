using PicPick.Commands;
using PicPick.Project;
using System;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{

    public class DestinationViewModel
    {
        public EventHandler OnDeleteClicked;

        public ICommand DeleteCommand { get; set; }

        public DestinationViewModel(PicPickProjectActivityDestination destination)
        {
            Destination = destination;
            PathViewModel = new PathBrowserViewModel(Destination);

            DeleteCommand = new RelayCommand(delegate { OnDeleteClicked?.Invoke(this, null); });
        }

        public PicPickProjectActivityDestination Destination { get; set; }

        public PathBrowserViewModel PathViewModel;
    }
}
