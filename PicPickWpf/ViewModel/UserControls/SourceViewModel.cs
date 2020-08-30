using PicPick.Models;
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
        private bool _useDateFrom;
        private DateTime _dateFrom;
        PicPickProjectActivity Activity;

        #endregion

        #region CTOR

        public SourceViewModel(PicPickProjectActivity activity)
        {
            Source = activity.Source;
            Activity = activity;
            activity.StateMachine.OnStateChanged += StateMachine_OnStateCompleted;

            PathViewModel = new PathBrowserViewModel(Source);
            Source.PropertyChanged += Source_PropertyChanged;

            InitSystemWatcher();
        }

        private void StateMachine_OnStateCompleted(object sender, EventArgs e)
        {
            OnPropertyChanged("SourceFilesStatus");
        }



        #endregion

        #region Private Methods

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

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "OnlyNewFiles":
                    break;
                case "FromDate":
                case "ToDate":
                    break;
                case "Path":
                    InitSystemWatcher();
                    break;
                default:
                    break;
            }
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
            get
            {
                if (Activity.StateMachine.CurrentState < PicPickState.READY_TO_RUN)
                {
                    if (!PathHelper.Exists(Source.Path))
                        return "Path not found";

                    return Activity.StateMachine.CurrentState.ToString();
                }

                return $"{Activity.FilesGraph.Files.Count()} files found";
            }
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
    }
}
