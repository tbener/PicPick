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
            Source = Activity.Source;
            Activity.StateMachine.OnStateChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SourceFilesStatus));
                OnPropertyChanged(nameof(BackgroundReadingInProgress));
            };

            Activity.OnActivityStateChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SourceFilesStatus));
                OnPropertyChanged(nameof(BackgroundReadingInProgress));
            };

            BackgroundReadingCommand = new RelayCommand(() => Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE));
            StopBackgroundReadingCommand = new RelayCommand(() => Activity.StateMachine.Stop());

            PathViewModel = new PathBrowserViewModel(new PathAdapter(Source));
            Source.PropertyChanged += Source_PropertyChanged;

            InitSystemWatcher();
        }

        #endregion

        #region Events

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Activity.State = ActivityState.NOT_STARTED;

            if (e.PropertyName.Equals("Path") || e.PropertyName.Equals(nameof(Source.IncludeSubFolders)))
                InitSystemWatcher();

            OnPropertyChanged(nameof(AdvancedFiltersHeader));

            switch (e.PropertyName)
            {
                case "OnlyNewFiles":
                    UpdateMapping(PicPickState.FILTERING);
                    break;
                case "FromDate":
                case "ToDate":
                    UpdateMapping(PicPickState.MAPPING);
                    break;
                default:
                    UpdateMapping(PicPickState.READING);
                    break;
            }

            OnPropertyChanged(nameof(SourceFilesStatus));
        }

        #endregion



        #region FileSystemWatcher


        private void InitSystemWatcher()
        {
            try
            {
                if (_fileSystemWatcher == null)
                {
                    _fileSystemWatcher = new FileSystemWatcher();
                    _fileSystemWatcher.Renamed += FileSystemWatcher_Changed;
                    _fileSystemWatcher.Created += FileSystemWatcher_Changed;
                    _fileSystemWatcher.Deleted += FileSystemWatcher_Changed;

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
            if (!BackgroundReadingEnabled)
                return;
            if (Activity.IsRunning)
                if (Activity.StateMachine.CurrentState > PicPickState.READY_TO_RUN)
                    return;

            Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE);
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            _timerCheckFiles.Stop();

            Activity.StateMachine.SetNeedRestart(PicPickState.READING);

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
                _fileSystemWatcher.Renamed -= FileSystemWatcher_Changed;
                _fileSystemWatcher.Created -= FileSystemWatcher_Changed;
                _fileSystemWatcher.Deleted -= FileSystemWatcher_Changed;
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
                if (Activity.StateMachine.CurrentState < PicPickState.READY_TO_RUN || Activity.StateMachine.NeedRestart)
                {
                    if (!PathHelper.Exists(Source.Path))
                        return "Path not found";

                    return Activity.StateMachine.IsRunning ? "Calculating..." : "Click refresh to calculate files";
                }

                return $"{Activity.FileGraph.Files.Count()} files found";
            }

        }

        public PathBrowserViewModel PathViewModel
        {
            get { return (PathBrowserViewModel)GetValue(PathViewModelProperty); }
            set { SetValue(PathViewModelProperty, value); }
        }

        public static readonly DependencyProperty PathViewModelProperty =
            DependencyProperty.Register("PathViewModel", typeof(PathBrowserViewModel), typeof(SourceViewModel), new PropertyMetadata(null));




        public bool BackgroundReadingInProgress
        {
            get
            {
                return Activity.StateMachine.IsRunning && !IsRunning;
            }
        }

        public override bool BackgroundReadingEnabled
        {
            get => base.BackgroundReadingEnabled;
            set
            {
                base.BackgroundReadingEnabled = value;
                if (base.BackgroundReadingEnabled && Activity.StateMachine.NeedRestart)
                    Activity.StateMachine.Restart(PicPickState.READING, BACKGROUND_END_STATE);
            }
        }

        private bool AdvancedFiltersHaveValue()
        {
            return Source.OnlyNewFiles
                    || Source.FromDate.Use
                    || Source.ToDate.Use;
        }

        public string AdvancedFiltersHeader
        {
            get
            {
                string header = "Advanced Filters";
                if (AdvancedFiltersHaveValue())
                    header += " *";

                return header;
            }
        }

        private bool? _isAdvancedFiltersExpanded;

        public bool IsAdvancedFiltersExpanded
        {
            get
            {
                if (!_isAdvancedFiltersExpanded.HasValue)
                    _isAdvancedFiltersExpanded = AdvancedFiltersHaveValue();
                return _isAdvancedFiltersExpanded.Value;
            }
            set
            {
                _isAdvancedFiltersExpanded = value;
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
