using PicPick.Commands;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.StateMachine;
using System.Windows;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{


    public class ExecutionViewModel : ActivityBaseViewModel
    {
        private PicPickState _runningEndState;

        public PicPickState FullRunEndState { get; set; } = PicPickState.DONE;
        public PicPickState AnalyzeEndState { get; set; } = PicPickState.READY_TO_RUN;

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand ShowPlanCommand { get; set; }

        #endregion

        public ExecutionViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            //StartCommand = new RelayCommand(Start, CanStart);
            StartCommand = new StartCommand(Activity);
            //StopCommand = new RelayCommand(Stop, CanStop);
            StopCommand = new StopCommand(Activity);
            ShowPlanCommand = StartCommand; // new RelayCommand(Analyze, CanStart);

            Activity.StateMachine.OnStateChanged += StateMachine_OnStateCompleted;
            //Activity.OnActivityStateChanged += (s, e) =>
            //{
            //    CommandManager.InvalidateRequerySuggested();
            //    //OnPropertyChanged(nameof(CanStop));
            //    //OnPropertyChanged(nameof(CanStart));
            //    OnPropertyChanged(nameof(StopCommand));
            //};

        }

        private void StateMachine_OnStateCompleted(object sender, CancellableEventArgs e)
        {
            

            switch (Activity.StateMachine.CurrentState)
            {
                case PicPickState.READY:
                    //if (BackgroundReadingEnabled && !Activity.StateMachine.IsRunning)
                    //    Activity.StateMachine.Start(BACKGROUND_END_STATE);
                    break;

                case PicPickState.READING:
                case PicPickState.MAPPING:
                case PicPickState.FILTERING:
                    //if (ProgressInfo.OperationCancelled)
                    //    Activity.State = ActivityState.NOT_STARTED;
                    break;

                // This could be a background reading, or a source refresh (which is treated the same as background reading)
                // OR, a real run.
                // If it's a real run, it could be either only mapping or a full run.
                case PicPickState.READY_TO_RUN:
                    if (Activity.StateMachine.IsRunning)
                    {
                        if (Properties.UserSettings.General.ShowPreviewWindow)
                        {
                            e.Cancel = !ShowMappingDialog(false);

                            if (e.Cancel)
                                Activity.IsRunning = false;
                        }
                    }
                    else if (Activity.IsRunning)
                    {
                        Activity.IsRunning = false;
                        ShowMappingDialog(true);
                    }

                    break;

                case PicPickState.RUNNING:
                    //if (ProgressInfo.OperationCancelled)
                    //    Activity.State = ActivityState.NOT_STARTED;
                    break;

                case PicPickState.DONE:
                    Activity.State = ActivityState.DONE;
                    if (Properties.UserSettings.General.ShowSummaryWindow)
                        ShowResultsDialog();
                    break;

                default:
                    break;
            }

            if (!Activity.StateMachine.IsRunning || ProgressInfo.OperationCancelled)
                Activity.IsRunning = false;
        }

        #region Execution


        private bool ShowMappingDialog(bool onlyMapping)
        {
            var vm = new MappingPlanViewModel(Activity, ProgressInfo);
            MessageBoxButton buttons = onlyMapping ? MessageBoxButton.OK : MessageBoxButton.OKCancel;
            var result = MessageBoxHelper.Show(vm, "Mapping Preview", buttons, !onlyMapping, out bool dontShowAgain);
            if (dontShowAgain)
                Properties.UserSettings.General.ShowPreviewWindow = false;
            return result == MessageBoxResult.OK;
        }

        private void ShowResultsDialog()
        {
            var vm = new MappingResultsViewModel(Activity);
            MessageBoxHelper.Show(vm, "Finished", MessageBoxButton.OK, true, out bool dontShowAgain);
            if (dontShowAgain)
                Properties.UserSettings.General.ShowSummaryWindow = false;
        }

        private void Analyze()
        {
            Run(PicPickState.READY_TO_RUN);
        }

        //private void Start()
        //{
        //    if (Properties.UserSettings.General.WarnDeleteSource)
        //        if (WarningsBeforeStart())
        //            Run(PicPickState.DONE);
        //}

        private void Run(PicPickState endState)
        {
            Activity.IsRunning = true;
            _runningEndState = endState;
            Activity.StateMachine.Start(endState);
        }


        public bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !IsRunning;
        }



        #endregion
    }
}
