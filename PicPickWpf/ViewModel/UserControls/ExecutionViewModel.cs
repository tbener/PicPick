using PicPick.Commands;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PicPick.ViewModel.UserControls
{
    

    public class ExecutionViewModel : ActivityBaseViewModel
    {
        private PicPickState _runningEndState;

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StopCommand { get; set; }

        #endregion

        public ExecutionViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            StartCommand = new RelayCommand(Start, CanStart);
            AnalyzeCommand = new RelayCommand(Analyze, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);

            Activity.StateMachine.OnStateChanged += StateMachine_OnStateCompleted;
            Activity.OnActivityStateChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanStop));
                OnPropertyChanged(nameof(CanStart));
            };

        }

        private void StateMachine_OnStateCompleted(object sender, CancellableEventArgs e)
        {
            switch (Activity.StateMachine.CurrentState)
            {
                case PicPickState.READY:
                    if (ProgressInfo.OperationCancelled)
                        Activity.State = ActivityState.NOT_STARTED;
                    else if (BackgroundReadingEnabled && !Activity.StateMachine.IsRunning)
                        Activity.StateMachine.Start(BACKGROUND_END_STATE);
                    break;

                case PicPickState.READING:
                case PicPickState.MAPPING:
                case PicPickState.FILTERING:
                    if (ProgressInfo.OperationCancelled)
                        Activity.State = ActivityState.NOT_STARTED;
                    break;

                // This could be a background reading, or a source refresh (which is treated the same as background reading)
                // OR, a real run.
                // If it's a real run, it could be either only mapping or a full run.
                case PicPickState.READY_TO_RUN:
                    if (IsRunning)
                    {
                        bool runOnlyMapping = _runningEndState == PicPickState.READY_TO_RUN;

                        if (Properties.UserSettings.General.ShowPreviewWindow || runOnlyMapping)
                        {
                            e.Cancel = !ShowMappingDialog(runOnlyMapping);

                            if (e.Cancel || runOnlyMapping)
                                Activity.State = ActivityState.NOT_STARTED;
                        }
                    }

                    break;

                case PicPickState.RUNNING:
                    break;

                case PicPickState.DONE:
                    Activity.State = ActivityState.DONE;
                    if (Properties.UserSettings.General.ShowSummaryWindow)
                        ShowResultsDialog();
                    break;
                
                default:
                    break;
            }
        }

        #region Execution

        private bool WarningsBeforeStart()
        {
            if (!Activity.DeleteSourceFiles)
                return true;

            if (!Properties.UserSettings.General.WarnDeleteSource)
                return true;

            string msgText = "The files will be deleted from the source folder if the operation will end successfully.\nDo you want to continue?";
            MessageBoxResult result = MessageBoxHelper.Show(msgText, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, out bool dontShowAgain);

            Properties.UserSettings.General.WarnDeleteSource = !dontShowAgain;

            return result == MessageBoxResult.Yes;
        }


        private bool ShowMappingDialog(bool onlyMapping)
        {
            var vm = new MappingPlanViewModel(Activity);
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

        private void Start()
        {
            if (Properties.UserSettings.General.WarnDeleteSource)
                if (WarningsBeforeStart())
                    Run(PicPickState.DONE);
        }

        private void Run(PicPickState endState)
        {
            Activity.State = ActivityState.RUNNING;
            _runningEndState = endState;
            Activity.StateMachine.Start(endState);
        }


        public bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !IsRunning;
        }


        public void Stop()
        {
            PicPickState jumpToState = Activity.StateMachine.CurrentState > PicPickState.READY_TO_RUN ? PicPickState.DONE : PicPickState.READY;
            Activity.StateMachine.Stop(jumpToState);
        }

        public bool CanStop()
        {
            return IsRunning;
        }


        #endregion
    }
}
