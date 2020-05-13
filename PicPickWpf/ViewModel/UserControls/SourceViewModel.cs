using PicPick.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class SourceViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        string _sourceFilesStatus;
        CancellationTokenSource ctsSourceCheck;

        private FileSystemWatcher _fileSystemWatcher;
        private System.Timers.Timer _timerCheckFiles;

        #endregion

        #region CTOR

        public SourceViewModel(PicPickProjectActivity activity)
        {
            Source = activity.Source;
            activity.OnActivityStateChanged += Activity_OnActivityStateChanged;

            Init();
        }

        

        #endregion

        #region Private Methods

        /// <summary>
        /// Associate the ViewModels to the inner objects
        /// </summary>
        private void Init()
        {
            // source
            PathViewModel = new PathBrowserViewModel(Source);
            Source.PropertyChanged += Source_PropertyChanged;

            // todo: make this async
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CheckSourceStatus();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            InitSystemWatcher();

        }

        private async void Activity_OnActivityStateChanged(PicPickProjectActivity activity, Core.ActivityStateChangedEventArgs e)
        {
            if (activity.State == ACTIVITY_STATE.DONE)
                await CheckSourceStatus();
        }

        private async Task CheckSourceStatus()
        {

            try
            {
                // Cancel previous operations
                ctsSourceCheck?.Cancel();

                // reset state
                SetFilesStatus(-1, "Reading...");

                if (!PathHelper.Exists(Source.Path))
                {
                    SetFilesStatus(0, "Path not found");
                    return;
                }

                // Create a new cancellations token and await a new task to count files
                ctsSourceCheck = new CancellationTokenSource();
                int count = await Task.Run(() => Source.FileList.Count);
                SetFilesStatus(count);
            }
            catch (OperationCanceledException)
            {
                // operation was canceled
                SetFilesStatus(-1);
            }
            catch (Exception ex)
            {
                // error in counting files. most probably because folder doesn't exist.
                SetFilesStatus(-1, ex.Message);
            }
        }

        private void SetFilesStatus(int num, string status = "")
        {
            SourceFilesStatus = num < 0 ? status : $"{num}  ({status})";
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

                if (Directory.Exists(Source.Path))
                {
                    _fileSystemWatcher.Path = Source.Path;
                    _fileSystemWatcher.IncludeSubdirectories = Source.IncludeSubFolders;
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

        private void FileSystemWatcher_OnMappingChanged(object sender, RenamedEventArgs e)
        {
            //
        }

        private void FileSystemWatcher_OnCountChanged(object sender, FileSystemEventArgs e)
        {
            _timerCheckFiles.Stop();
            SetFilesStatus(-1, "changes detected...");
            _timerCheckFiles.Start();
        }

        private async void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await CheckSourceStatus();

            InitSystemWatcher();
        }

        public void Dispose()
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            Source = null;

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _fileSystemWatcher.Renamed -= FileSystemWatcher_OnMappingChanged;
                _fileSystemWatcher.Created -= FileSystemWatcher_OnCountChanged;
                _fileSystemWatcher.Deleted -= FileSystemWatcher_OnCountChanged;
                _fileSystemWatcher = null;
            }
        }

        #endregion

        #region Public Properties

        public PicPickProjectActivitySource Source { get; set; }

        public string SourceFilesStatus
        {
            get => _sourceFilesStatus;
            set
            {
                _sourceFilesStatus = value;
                OnPropertyChanged("SourceFilesStatus");
            }
        }

        public PathBrowserViewModel PathViewModel
        {
            get { return (PathBrowserViewModel)GetValue(PathViewModelProperty); }
            set { SetValue(PathViewModelProperty, value); }
        }

        public static readonly DependencyProperty PathViewModelProperty =
            DependencyProperty.Register("PathViewModel", typeof(PathBrowserViewModel), typeof(SourceViewModel), new PropertyMetadata(null));

        #endregion
    }
}
