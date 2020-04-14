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

namespace PicPick.ViewModel.UserControls
{
    public class ActivityViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        //CancellationTokenSource cts;

        //PicPickProjectActivity _activity;
        string _sourceFilesStatus;
        CancellationTokenSource ctsSourceCheck;

        private bool _keepDestinationsAbsolute;
        private bool _isRunning;

        private FileSystemWatcher _fileSystemWatcher;
        private System.Timers.Timer _timerCheckFiles;

        #endregion

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand AddDestinationCommand { get; set; }

        #endregion

        #region CTOR

        public ActivityViewModel(PicPickProjectActivity activity)
        {

            AddDestinationCommand = new RelayCommand(AddDestination);
            StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
            AnalyzeCommand = new AsyncRelayCommand(AnalyzeAsync, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);

            Activity = activity;
            InitActivity();

            ProgressInfo = new ProgressInformation();
            //((Progress<ProgressInformation>)ProgressInfo.Progress).ProgressChanged += ActivityViewModel_ProgressChanged;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Associate the ViewModels to the inner objects
        /// </summary>
        private void InitActivity()
        {
            // source
            SourceViewModel = new PathBrowserViewModel(Activity.Source);
            Activity.Source.PropertyChanged += Source_PropertyChanged;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CheckSourceStatus();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // destinations
            DestinationViewModelList = new ObservableCollection<DestinationViewModel>();
            foreach (PicPickProjectActivityDestination dest in Activity.DestinationList)
                AddDestinationViewModel(dest);

            Activity.OnActivityStateChanged += Activity_OnActivityStateChanged;

            InitSystemWatcher();

        }



        private async Task CheckSourceStatus()
        {

            try
            {
                // Cancel previous operations
                ctsSourceCheck?.Cancel();

                // reset state
                SourceFilesStatus = "Reading...";

                if (!PathHelper.Exists(Activity.Source.Path))
                {
                    SourceFilesStatus = "Path not found";
                    return;
                }

                // Create a new cancellations token and await a new task to count files
                ctsSourceCheck = new CancellationTokenSource();
                int count = await Task.Run(() => Activity.Source.FileList.Count);
                SourceFilesStatus = $"{count} files found";
            }
            catch (OperationCanceledException)
            {
                // operation was canceled
                SourceFilesStatus = "";
            }
            catch (Exception)
            {
                // error in counting files. most probably because folder doesn't exist.
                SourceFilesStatus = "---";
            }
        }

        private void InitSystemWatcher()
        {
            try
            {
                if (_fileSystemWatcher == null)
                {
                    _fileSystemWatcher = new FileSystemWatcher();
                    _fileSystemWatcher.Renamed += FileSystemWatcher_OnMappingChanged;
                    _fileSystemWatcher.Created += FileSystemWatcher_OnCountChanged;
                    _fileSystemWatcher.Deleted += FileSystemWatcher_OnCountChanged;

                    _timerCheckFiles = new System.Timers.Timer(1000);
                    _timerCheckFiles.Elapsed += TimerCheckFiles_Elapsed;
                    _timerCheckFiles.AutoReset = false;
                }
                else
                {
                    _fileSystemWatcher.EnableRaisingEvents = false;
                }

                if (Directory.Exists(Activity.Source.Path))
                {
                    _fileSystemWatcher.Path = Activity.Source.Path;
                    _fileSystemWatcher.IncludeSubdirectories = Activity.Source.IncludeSubFolders;
                    _fileSystemWatcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
            }
        }

        private async void TimerCheckFiles_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await CheckSourceStatus();
        }

        private void timerCallback(object state)
        {
            throw new NotImplementedException();
        }

        private void FileSystemWatcher_OnMappingChanged(object sender, RenamedEventArgs e)
        {
            //
        }

        private void FileSystemWatcher_OnCountChanged(object sender, FileSystemEventArgs e)
        {
            _timerCheckFiles.Stop();
            SourceFilesStatus = "Changes detected...";
            _timerCheckFiles.Start();
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
            MappingBaseViewModel vm;
            bool dontShowAgain;

            switch (activity.State)
            {
                case ACTIVITY_STATE.NOT_STARTED:
                    break;
                case ACTIVITY_STATE.ANALYZING:
                    break;
                case ACTIVITY_STATE.ANALYZED:
                    if (!Properties.UserSettings.General.ShowPreviewWindow && !Activity.RunMode_AnalyzeOnly)
                        return;
                    vm = new MappingPlanViewModel(Activity);
                    var result = MessageBoxHelper.Show(vm, "Mapping Preview", MessageBoxButton.OKCancel, out dontShowAgain);
                    if (dontShowAgain)
                        Properties.UserSettings.General.ShowPreviewWindow = false;
                    if (result != MessageBoxResult.OK)
                        e.Cancel = true;
                    break;
                case ACTIVITY_STATE.RUNNING:
                    break;
                case ACTIVITY_STATE.DONE:
                    await CheckSourceStatus();
                    if (!Properties.UserSettings.General.ShowSummaryWindow)
                        return;
                    vm = new MappingResultsViewModel(Activity);
                    MessageBoxHelper.Show(vm, "Finished", MessageBoxButton.OK, out dontShowAgain);
                    if (dontShowAgain)
                        Properties.UserSettings.General.ShowSummaryWindow = false;
                    break;
                default:
                    break;
            }
        }

        public async Task AnalyzeAsync()
        {
            Activity.RunMode_AnalyzeOnly = true;
            await StartAsync();
            Activity.RunMode_AnalyzeOnly = false;
        }

        public async Task StartAsync()
        {
            ProgressInfo.Reset();
            ProgressInfo.Report();

            _log.Info("Start");

            if (!WarningsBeforeStart())
                return;

            try
            {
                ProgressInfo.RenewToken();
                IsRunning = true;
                EventAggregatorHelper.PublishActivityStarted();

                await Activity.ExecuteAsync(ProgressInfo);

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


        private void Stop()
        {
            ProgressInfo.Cancel();
        }

        private bool CanStop()
        {
            return IsRunning;
        }



        public ProgressInformation ProgressInfo { get; set; }

        #endregion

        private async void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await CheckSourceStatus();

            InitSystemWatcher();
        }

        private void OnDestinationDelete(object sender, EventArgs e)
        {
            DestinationViewModel vm = sender as DestinationViewModel;
            if (vm == null) return;
            if (vm.Destination != null)
                Activity.DestinationList.Remove(vm.Destination);
            DestinationViewModelList.Remove(vm);
            if (Activity.DestinationList.Count == 1)
                Activity.DestinationList.First().Active = true;
        }

        private void AddDestination()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
            {
                Path = "",
                Template = "dd-yy"
            };
            Activity.DestinationList.Add(dest);
            AddDestinationViewModel(dest);
        }

        private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        {
            var vm = new DestinationViewModel(dest, Activity.Source);
            vm.OnDeleteClicked += OnDestinationDelete;
            DestinationViewModelList.Add(vm);
        }

        public void Dispose()
        {
            Activity.Source.PropertyChanged -= Source_PropertyChanged;
            Activity = null;

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _fileSystemWatcher.Renamed -= FileSystemWatcher_OnMappingChanged;
                _fileSystemWatcher.Created -= FileSystemWatcher_OnCountChanged;
                _fileSystemWatcher.Deleted -= FileSystemWatcher_OnCountChanged;
                _fileSystemWatcher = null;
            }
        }


        public PicPickProjectActivity Activity { get; set; }

        public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }



        public PathBrowserViewModel SourceViewModel
        {
            get { return (PathBrowserViewModel)GetValue(SourceViewModelProperty); }
            set { SetValue(SourceViewModelProperty, value); }
        }

        public static readonly DependencyProperty SourceViewModelProperty =
            DependencyProperty.Register("SourceViewModel", typeof(PathBrowserViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));

        public string SourceFilesStatus
        {
            get => _sourceFilesStatus;
            set
            {
                _sourceFilesStatus = value;
                OnPropertyChanged("SourceFilesStatus");
            }
        }



        public bool KeepDestinationsAbsolute
        {
            get { return DestinationViewModelList.FirstOrDefault().Destination.KeepAbsolute; }
            set
            {
                _keepDestinationsAbsolute = value;
                foreach (DestinationViewModel destvm in DestinationViewModelList)
                {
                    destvm.Destination.KeepAbsolute = _keepDestinationsAbsolute;
                }
            }
        }

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
