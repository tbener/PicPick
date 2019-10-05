using PicPick.Commands;
using PicPick.Models;
using PicPick.ViewModel;
using System;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{

    public class DestinationViewModel : BaseViewModel
    {
        private const string DATE_NOTATIONS_HELP_TEXT =
"dd \t Day \t \t \t 08\n" +
"ddd \t Short Day Name \t \t Sun\n" +
"dddd \t Full Day Name \t \t Sunday\n" +
"hh \t 2 digit hour      \t  \t 09\n" +
"HH \t 2 digit hour(24 hour) \t 21\n" +
"mm \t 2 digit minute \t \t 08\n" +
"MM \t Month \t \t \t 04\n" +
"MMM \t Short Month name \t Apr\n" +
"MMMM \t Month name \t \t April\n" +
"ss \t seconds \t \t 59\n" +
"tt \t AM/PM \t \t \t PM\n" +
"yy \t 2 digit year \t \t 07\n" +
"yyyy \t 4 digit year \t \t 2007";

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

        public string TemplateToolTip
        {
            get
            {
                string tooltip = "Use standard date symbols (e.g. dd-MM-yy) to create the folder according to the image date";
                return tooltip + "\n\n" + DATE_NOTATIONS_HELP_TEXT;
            }
        }
    }
}
