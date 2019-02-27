using PicPick.Commands;
using PicPick.Project;
using PicPick.ViewModel;
using System;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{

    public class DestinationViewModel : BaseViewModel
    {
        public EventHandler OnDeleteClicked;

        public ICommand DeleteCommand { get; set; }

        public DestinationViewModel(PicPickProjectActivityDestination destination)
        {
            Destination = destination;
            PathViewModel = new PathBrowserViewModel(Destination);

            DeleteCommand = new RelayCommand(delegate { OnDeleteClicked?.Invoke(this, null); });

            Destination.PropertyChanged += Destination_PropertyChanged;
        }

        private void Destination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Template"))
                OnPropertyChanged("TemplatePreview");
        }

        public PicPickProjectActivityDestination Destination { get; set; }

        public PathBrowserViewModel PathViewModel { get; set; }

        public string TemplatePreview
        {
            get
            {
                return Destination.GetTemplatePath(DateTime.Now);
            }
        }
    }
}
