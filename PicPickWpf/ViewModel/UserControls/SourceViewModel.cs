using PicPick.Commands;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.Models.Mapping;
using PicPick.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TalUtils;

namespace PicPick.ViewModel.UserControls
{
    public class SourceViewModel : ActivityBaseViewModel
    {
        #region Private Members

        private FileSystemWatcher _fileSystemWatcher;

        private System.Timers.Timer _timerCheckFiles;
        private bool _enabled = true;

        #endregion

        #region Commands

        public ICommand BackgroundReadingCommand { get; set; }
        public ICommand StopBackgroundReadingCommand { get; set; }

        #endregion

        #region CTOR

        public SourceViewModel(IActivity activity, IProgressInformation progressInfo) : base(activity, progressInfo)
        {
            Source = activity.Source;
            Activity.StateMachine.OnStateChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SourceFilesStatus));
                OnPropertyChanged(nameof(BackgroundReadingInProgress));
            };

            Activity.OnActivityStateChanged += (s, e) =>
            {
                Enabled = !IsRunning;
                OnPropertyChanged(nameof(SourceFilesStatus));
                OnPropertyChanged(nameof(BackgroundReadingInProgress));
            };

            BackgroundReadingCommand = new RelayCommand(() => Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE));
            StopBackgroundReadingCommand = new RelayCommand(() => Activity.StateMachine.Stop());

            PathViewModel = new PathBrowserViewModel(Source);
            Source.PropertyChanged += Source_PropertyChanged;

            InitSystemWatcher();
        }

        #endregion

        #region Events

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Activity.State = ActivityState.NOT_STARTED;

            if (e.PropertyName.Equals("Path"))
                InitSystemWatcher();

            if (!BackgroundReadingEnabled) return;

            switch (e.PropertyName)
            {
                case "OnlyNewFiles":
                    Activity.StateMachine.Restart(PicPickState.FILTERING, BACKGROUND_END_STATE);
                    break;
                case "FromDate":
                case "ToDate":
                    Activity.StateMachine.Restart(PicPickState.MAPPING, BACKGROUND_END_STATE);
                    break;
                default:
                    Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE);
                    break;
            }

        }

        #endregion


        #region FileSystemWatcher


        private void InitSystemWatcher()
        {
            return;
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

        private void TimerCheckFiles_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnPropertyChanged("SourceFilesStatus");
        }

        private void FileSystemWatcher_OnMappingChanged(object sender, RenamedEventArgs e)
        {
            //
        }

        private void FileSystemWatcher_OnCountChanged(object sender, FileSystemEventArgs e)
        {
            _timerCheckFiles.Stop();

            // todo:
            // need to mark that the FileGraph is not updated
            // we can update the counter according to the change
            // and set the timer for re-read and map the files

            _timerCheckFiles.Start();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
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

            base.Dispose();
        }

        #endregion

        #region Public Properties

        public PicPickProjectActivitySource Source { get; set; }

        public string SourceFilesStatus
        {
            get
            {
                if (Activity.StateMachine.CurrentState < PicPickState.READY_TO_RUN)
                {
                    if (!PathHelper.Exists(Source.Path))
                        return "Path not found";

                    return Activity.StateMachine.IsRunning ? "Calculating..." : "Click refresh to calculate files";
                }

                return $"{Activity.FilesGraph.Files.Count()} files found";
            }

        }

        public PathBrowserViewModel PathViewModel
        {
            get { return (PathBrowserViewModel)GetValue(PathViewModelProperty); }
            set { SetValue(PathViewModelProperty, value); }
        }

        public static readonly DependencyProperty PathViewModelProperty =
            DependencyProperty.Register("PathViewModel", typeof(PathBrowserViewModel), typeof(SourceViewModel), new PropertyMetadata(null));


        public bool Enabled
        {
            get => _enabled;
            internal set
            {
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }


        public bool BackgroundReadingInProgress
        {
            get
            {
                return Activity.StateMachine.IsRunning && !IsRunning;
            }
        }

        #region Dates Properties

        private DateComplex SetDate(DateComplex dateProp, DateTime date, bool use)
        {
            if (dateProp == null)
                dateProp = new DateComplex();

            dateProp.Use = use;
            dateProp.Date = date;

            return dateProp;
        }

        public bool UseFromDate
        {
            get
            {
                return Source.FromDate != null && Source.FromDate.Use;
            }
            set
            {
                Source.FromDate = SetDate(Source.FromDate, DateFrom, value);
                OnPropertyChanged("DateFrom");
            }
        }

        public DateTime DateFrom
        {
            get
            {
                return Source.FromDate == null ? DateTime.Today.AddMonths(-1) : Source.FromDate.Date;
            }
            set
            {
                Source.FromDate = SetDate(Source.FromDate, value, true);
                OnPropertyChanged("UseFromDate");
            }
        }

        public bool UseToDate
        {
            get
            {
                return Source.ToDate != null && Source.ToDate.Use;
            }
            set
            {
                Source.ToDate = SetDate(Source.ToDate, DateTo, value);
                OnPropertyChanged("DateTo");
            }
        }

        public DateTime DateTo
        {
            get
            {
                return Source.ToDate == null ? DateTime.Today : Source.ToDate.Date;
            }
            set
            {
                Source.ToDate = SetDate(Source.ToDate, value, true);
                OnPropertyChanged("UseToDate");
            }
        }

        #endregion

        #endregion
    }
}
