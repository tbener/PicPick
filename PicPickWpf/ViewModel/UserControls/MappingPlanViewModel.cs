using PicPick.Commands;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.StateMachine;
using PicPick.ViewModel.UserControls.Mapping;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    public class MappingPlanViewModel : MappingBaseViewModel
    {
        public ICommand RefreshCommand { get; set; }

        public MappingPlanViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            // Source Pane
            SourceFilesDeleteWarning = Activity.DeleteSourceFiles ? "The files will be deleted!" : "The files won't be deleted.";

            RefreshCommand = new RelayCommand(() => Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE));

            Activity.StateMachine.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "NeedRestart")
                    if (Activity.StateMachine.NeedRestart)
                    {
                        OnPropertyChanged(nameof(NeedUpdate));
                        OnPropertyChanged(nameof(CanRefresh));
                        OnPropertyChanged(nameof(NeedUpdateWarning));
                    }
            };

            Activity.StateMachine.OnStateChanged += (s, e) =>
            {
                if (Activity.StateMachine.CurrentState == PicPickState.READY_TO_RUN)
                {
                    OnPropertyChanged("SourceFoundFiles");
                    OnPropertyChanged("DestinationList");
                    OnPropertyChanged(nameof(NeedUpdate));
                }
            };
        }


        public string SourceFilesDeleteWarning { get; set; }

        public bool NeedUpdate => Activity.StateMachine.NeedRestart;

        public bool CanRefresh => NeedUpdate && !Activity.IsRunning;

        public string NeedUpdateWarning => "File System changes were detected. " + (CanRefresh ? "Click the Refresh button to update the plan." : "It is recommended to click Cancel and start over.");

        public string Icon
        {
            get
            {
                return "/PicPickUI;component/Resources/mapping.ico";
            }
        }

    }
}
