using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PicPick.Commands;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using TalUtils;
using PicPick.ViewModel.Dialogs;
using PicPick.View.Dialogs;
using System.IO;
using PicPick.ViewModel.UserControls.Mapping;
using System.Diagnostics;
using PicPick.StateMachine;

namespace PicPick.ViewModel.UserControls
{
    public class ActivityViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        //CancellationTokenSource cts;

        //PicPickProjectActivity _activity;
        //string _sourceFilesStatus;
        //CancellationTokenSource ctsSourceCheck;

        private bool _isRunning;
        private bool _readInBackground;

        private readonly PicPickState _backgroundEndState = PicPickState.READY_TO_RUN;

        #endregion

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand BackgroundReadingCommand { get; set; }

        #endregion

        #region CTOR

        public ActivityViewModel(PicPickProjectActivity activity)
        {

            StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
            AnalyzeCommand = new AsyncRelayCommand(AnalyzeAsync, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            BackgroundReadingCommand = new RelayCommand(() => Activity.StateMachine.Restart(PicPickState.READING, _backgroundEndState));

            Activity = activity;

            ProgressInfo = new ProgressInformation();

            Activity.StateMachine.ProgressInfo = ProgressInfo;
            Activity.StateMachine.OnStateChanged += StateMachine_OnStateCompleted;
            Activity.Source.PropertyChanged += Source_PropertyChanged;

            SourceViewModel = new SourceViewModel(Activity);
            SourceViewModel.BackgroundReadingCommand = BackgroundReadingCommand;
            DestinationListViewModel = new DestinationListViewModel(Activity);

            Activity.OnActivityStateChanged += Activity_OnActivityStateChanged;

            ReadInBackground = true;
            Activity.StateMachine.Start(_backgroundEndState);
        }


        #endregion

        #region Private Methods

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ReadInBackground) return;

            switch (e.PropertyName)
            {
                case "OnlyNewFiles":
                    Activity.StateMachine.Restart(PicPickState.FILTERING, _backgroundEndState);
                    break;
                case "FromDate":
                case "ToDate":
                    Activity.StateMachine.Restart(PicPickState.MAPPING, _backgroundEndState);
                    break;
                default:
                    Activity.StateMachine.Restart(PicPickState.READING, _backgroundEndState);
                    break;
            }
        }

        private void StateMachine_OnStateCompleted(object sender, EventArgs e)
        {
            switch (Activity.StateMachine.CurrentState)
            {
                case PicPickState.READY:
                    //if (ReadInBackground)
                    //    Activity.StateMachine.Start(_backgroundEndState);
                    break;
                case PicPickState.READY_TO_RUN:
                    if (!IsRunning)
                        ProgressInfo.Finished();
                    //if (!Properties.UserSettings.General.ShowPreviewWindow && !Activity.RunMode_AnalyzeOnly)
                    //    return;
                    //vm = new MappingPlanViewModel(Activity);
                    //var result = MessageBoxHelper.Show(vm, "Mapping Preview", MessageBoxButton.OKCancel, out bool dontShowAgain);
                    //if (dontShowAgain)
                    //    Properties.UserSettings.General.ShowPreviewWindow = false;
                    //if (result != MessageBoxResult.OK)
                    //    e.Cancel = true;
                    break;
                case PicPickState.DONE:
                    //if (!Properties.UserSettings.General.ShowSummaryWindow)
                    //    return;
                    //var vm = new MappingResultsViewModel(Activity);
                    //MessageBoxHelper.Show(vm, "Finished", MessageBoxButton.OK, out bool dontShowAgain);
                    //if (dontShowAgain)
                    //    Properties.UserSettings.General.ShowSummaryWindow = false;
                    break;
                default:
                    break;
            }
            OnPropertyChanged(nameof(CanStop));
        }

        #endregion

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


        private async void Activity_OnActivityStateChanged(PicPickProjectActivity activity, ActivityStateChangedEventArgs e)
        {
            //MappingBaseViewModel vm;
            //bool dontShowAgain;

            //switch (activity.State)
            //{
            //    case ACTIVITY_STATE.NOT_STARTED:
            //        break;
            //    case ACTIVITY_STATE.ANALYZING:
            //        break;
            //    case ACTIVITY_STATE.ANALYZED:
            //        if (!Properties.UserSettings.General.ShowPreviewWindow && !Activity.RunMode_AnalyzeOnly)
            //            return;
            //        vm = new MappingPlanViewModel(Activity);
            //        var result = MessageBoxHelper.Show(vm, "Mapping Preview", MessageBoxButton.OKCancel, out dontShowAgain);
            //        if (dontShowAgain)
            //            Properties.UserSettings.General.ShowPreviewWindow = false;
            //        if (result != MessageBoxResult.OK)
            //            e.Cancel = true;
            //        break;
            //    case ACTIVITY_STATE.RUNNING:
            //        break;
            //    case ACTIVITY_STATE.DONE:
            //        if (!Properties.UserSettings.General.ShowSummaryWindow)
            //            return;
            //        vm = new MappingResultsViewModel(Activity);
            //        MessageBoxHelper.Show(vm, "Finished", MessageBoxButton.OK, out dontShowAgain);
            //        if (dontShowAgain)
            //            Properties.UserSettings.General.ShowSummaryWindow = false;
            //        break;
            //    default:
            //        break;
            //}
        }

        public async Task AnalyzeAsync()
        {
            await RunAsync(true);
        }

        public async Task StartAsync()
        {
            _log.Info("Start");

            if (!WarningsBeforeStart())
                return;

            await RunAsync(false);

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

        private async Task RunAsync(bool onlyMapping)
        {
            try
            {
                IsRunning = true;
                EventAggregatorHelper.PublishActivityStarted();

                await Activity.StateMachine.StartAsync(PicPickState.READY_TO_RUN);
                if (Properties.UserSettings.General.ShowPreviewWindow || onlyMapping)
                {
                    if (!ShowMappingDialog(onlyMapping) || onlyMapping)
                        return;
                }

                await Activity.StateMachine.StartAsync(PicPickState.DONE);
                IsRunning = false;

                // It would be nice to perform an asynchronous reading while display the results dialog,
                //   but since we're currently sharing the same FileMap object we must do this synchronously.
                //new Task(() => Activity.StateMachine.Restart(PicPickState.READY, _backgroundEndState)).Start();

                if (Properties.UserSettings.General.ShowSummaryWindow)
                    ShowResultsDialog();

                Activity.StateMachine.Restart(PicPickState.READY, _backgroundEndState);
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
                MessageBoxHelper.Show(ex);
            }
            finally
            {
                IsRunning = false;
                await Task.Run(() => ProgressInfo.Finished());
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !IsRunning;
        }


        public void Stop()
        {
            Activity.StateMachine.Stop();
            Thread.Sleep(1000);
            ProgressInfo.Reset();
        }

        public bool CanStop()
        {
            return Activity.StateMachine.IsRunning;
        }

        public bool ReadInBackground
        {
            get => _readInBackground;
            set
            {
                _readInBackground = value;
                if (_readInBackground)
                    Activity.StateMachine.Restart(PicPickState.READING, _backgroundEndState);
                else
                    Activity.StateMachine.Stop();
            }
        }

        public ProgressInformation ProgressInfo { get; set; }

        #endregion


        //private void OnDestinationDelete(object sender, EventArgs e)
        //{
        //    DestinationViewModel vm = sender as DestinationViewModel;
        //    if (vm == null) return;
        //    if (vm.Destination != null)
        //        Activity.DestinationList.Remove(vm.Destination);
        //    DestinationViewModelList.Remove(vm);
        //    if (Activity.DestinationList.Count == 1)
        //        Activity.DestinationList.First().Active = true;
        //}

        //private void AddDestination()
        //{
        //    PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
        //    {
        //        Path = "",
        //        Template = "dd-yy"
        //    };
        //    Activity.DestinationList.Add(dest);
        //    AddDestinationViewModel(dest);
        //}

        //private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        //{
        //    var vm = new DestinationViewModel(dest, Activity.Source);
        //    vm.OnDeleteClicked += OnDestinationDelete;
        //    DestinationViewModelList.Add(vm);
        //}

        public void Dispose()
        {
            Activity = null;
        }


        public PicPickProjectActivity Activity { get; set; }

        //public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }




        public SourceViewModel SourceViewModel
        {
            get { return (SourceViewModel)GetValue(SourceViewModelProperty); }
            set { SetValue(SourceViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceViewModelProperty =
            DependencyProperty.Register("SourceViewModel", typeof(SourceViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        public DestinationListViewModel DestinationListViewModel
        {
            get { return (DestinationListViewModel)GetValue(DestinationListViewModelProperty); }
            set { SetValue(DestinationListViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DestinationListViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DestinationListViewModelProperty =
            DependencyProperty.Register("DestinationListViewModel", typeof(DestinationListViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }


    }
}
