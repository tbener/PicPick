using PicPick.Models;
using PicPick.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TalUtils;
using System.Windows;
using System.IO;
using PicPick.Helpers;
using PicPick.Core;
using log4net;
using PicPick.ViewModel.Dialogs;
using PicPick.View.Dialogs;
using PicPick.ViewModel.UserControls;
using PicPick.Models.Interfaces;

namespace PicPick.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel, IDisposable
    {

        private IActivity _currentActivity;
        private Dictionary<IActivity, ActivityViewModel> _activityViewModels = new Dictionary<IActivity, ActivityViewModel>();

        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddActivityCommand { get; set; }
        public ICommand DeleteActivityCommand { get; set; }
        public ICommand BrowseLogFileCommand { get; set; }
        public ICommand SendMailDialogCommand { get; set; }
        public ICommand OpenPageCommand { get; set; }
        public ICommand DuplicateActivityCommand { get; set; }
        public ICommand MoveActivityDownCommand { get; set; }
        public ICommand MoveActivityUpCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StopCommand { get; set; }

        public MainWindowViewModel()
        {

            OpenFileCommand = new RelayCommand(OpenFileWithDialog);
            SaveCommand = new SaveCommand();
            AddActivityCommand = new RelayCommand(CreateNewActivity);
            DeleteActivityCommand = new RelayCommand(DeleteActivity, () => CurrentProject.ActivityList.Count > 1);
            BrowseLogFileCommand = new RelayCommand(BrowseLogFile);
            SendMailDialogCommand = new RelayCommand(SendMailDialog);
            OpenPageCommand = new RelayCommand(OpenPicPickPage);
            DuplicateActivityCommand = new RelayCommand(DuplicateActivity);
            MoveActivityDownCommand = new RelayCommand(MoveActivityDown, CanMoveActivityDown);
            MoveActivityUpCommand = new RelayCommand(MoveActivityUp, CanMoveActivityUp);

            LogFile = LogManager.GetRepository().GetAppenders().OfType<log4net.Appender.RollingFileAppender>().FirstOrDefault()?.File;

            ProjectLoader.OnSaveEventHandler += (s, e) => UpdateFileName();

            try
            {
                _log.Info("Loading file...");
                if (string.IsNullOrEmpty(Properties.Settings.Default.LastFile) || !File.Exists(Properties.Settings.Default.LastFile))
                    LoadOrCreateNew();
                else
                    OpenFile(Properties.Settings.Default.LastFile);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error loading data file");
            }

            CurrentActivity = CurrentProject.ActivityList.FirstOrDefault();

            ApplicationService.Instance.EventAggregator.GetEvent<GotDirtyEvent>().Subscribe(OnGotDirty);
            ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Subscribe(OnFileExistsAskEvent);
            ApplicationService.Instance.EventAggregator.GetEvent<FileErrorEvent>().Subscribe(OnFileErrorEvent);

        }

        private void InitIsDirty()
        {
            CurrentProject.GetIsDirtyInstance().OnGotDirty += (s, e) => OnGotDirty();
            CurrentProject.IsDirty = false;
        }

        private void OnFileErrorEvent(FileErrorEventArgs e)
        {
            string text = $"Error while copying to {e.DestinationFile.GetFullName()}";
            text += $"\n{e.DestinationFile.Exception}";
            text += "\n\nDo you want to continue to the next file?";

            MessageBoxResult result = MessageBoxHelper.Show(text, "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            e.Cancel = result == MessageBoxResult.No;

            //MessageViewModel messageViewModel = new MessageViewModel(text, "Error", MessageBoxButton.YesNo, MessageBoxImage.Error, false);
            //MessageView messageView = new MessageView();
            //messageView.DataContext = messageViewModel;

            //messageViewModel.CloseDialog = () =>
            //{
            //    messageView.Close();
            //};

            //messageView.Owner = Application.Current.MainWindow;
            //messageView.ShowDialog();

            //e.Cancel = messageViewModel.DialogResult == MessageBoxResult.No;
        }

        private void DuplicateActivity()
        {
            PicPickProjectActivity activity = CurrentActivity.Clone(CurrentActivity.Name + " 2");
            CurrentProject.ActivityList.Insert(CurrentProject.ActivityList.IndexOf(CurrentActivity) + 1, activity);
            CurrentActivity = activity;
        }

        private void CreateNewActivity()
        {
            string activityName = (string)Application.Current.FindResource("ppk_new_activity");
            PicPickProjectActivity activity = PicPickProjectActivity.CreateNew(activityName);
            CurrentProject.ActivityList.Add(activity);
            CurrentActivity = activity;
        }

        private bool CanMoveActivityUp(object obj)
        {
            return CurrentActivity != null && CurrentProject.ActivityList.IndexOf(CurrentActivity) > 0;
        }

        private void MoveActivityUp()
        {
            int idx = CurrentProject.ActivityList.IndexOf(CurrentActivity);
            CurrentProject.ActivityList.Move(idx, idx - 1);
        }

        private bool CanMoveActivityDown(object obj)
        {
            return CurrentActivity != null && CurrentProject.ActivityList.IndexOf(CurrentActivity) < CurrentProject.ActivityList.Count-1;
        }

        private void MoveActivityDown()
        {
            int idx = CurrentProject.ActivityList.IndexOf(CurrentActivity);
            CurrentProject.ActivityList.Move(idx, idx + 1);
        }


        private void OpenPicPickPage()
        {
            ExplorerHelper.BrowseUrl("https://paper.dropbox.com/doc/PicPick--Am102Con1rJAho344HwatyKxAQ-zN60ducngrWz7nPn3MeZO");
        }

        private void SendMailDialog()
        {
            Clipboard.SetText("tbener+hauuclggg7taowshsqsu@boards.trello.com");
            Msg.Show("To add a card to the PicPick board, just send an email with a fully descriptive subject and body to:\n\ntbener+hauuclggg7taowshsqsu@boards.trello.com\n\nThe email address was copied to your Clipboard.\nThanks for you feedback.");
        }

        private void BrowseLogFile()
        {
            ExplorerHelper.OpenContainingFolder(LogFile);
        }

        private void OnGotDirty()
        {
            OnPropertyChanged("WindowTitle");
        }



        private void DeleteActivity()
        {
            if (CurrentProject.ActivityList.Count == 1)
            {
                Msg.Show("You cannot delete the last Activity");
                return;
            }

            if (Msg.ShowQ($"Are you sure you want to delete {CurrentActivity.Name}?"))
            {
                // remove view model from dictionary
                if (_activityViewModels.ContainsKey(CurrentActivity))
                {
                    _activityViewModels[CurrentActivity].Dispose();
                    _activityViewModels.Remove(CurrentActivity);
                }

                // remove from activity list (also sets CurrentActivity to null)
                int selIndex = CurrentProject.ActivityList.IndexOf(CurrentActivity);
                CurrentProject.ActivityList.Remove(CurrentActivity);

                // set selected activity
                if (CurrentProject.ActivityList.Count <= selIndex)
                    selIndex--;
                CurrentActivity = CurrentProject.ActivityList[selIndex];
            }
        }

        public bool CheckSaveIfDirty(string msg)
        {
            if (!CurrentProject.IsDirty)
                return true;
            var result = MessageBoxHelper.Show(msg, "PicPick file is not saved", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
                return false;
            if (result == MessageBoxResult.Yes)
                ProjectLoader.Save();
            return true;
        }

        private void LoadOrCreateNew()
        {
            if (!ProjectLoader.LoadDefault())
            {
                PicPickProject project = PicPickProject.CreateNew("Default", (string)Application.Current.FindResource("ppk_first_activity"));
                ProjectLoader.Create(project);
            }
            InitIsDirty();
        }

        void OpenFileWithDialog()
        {
            if (!CheckSaveIfDirty("Current project is not saved. Would you like to save before opening a new one?"))
                return;

            string file = ProjectLoader.FileName;
            if (FileSystemDialogHelper.BrowseOpenFileByExtensions(new[] { "picpick" }, true, ref file))
            {
                OpenFile(file);
            }

        }

        private void OpenFile(string file)
        {
            if (!ProjectLoader.Load(file)) return;
            // set the current activity to refresh the window
            CurrentActivity = CurrentProject.ActivityList.FirstOrDefault();
            UpdateFileName();
            InitIsDirty();
        }

        private void UpdateFileName()
        {
            OnPropertyChanged("CurrentProject");
            Properties.Settings.Default.LastFile = ProjectLoader.FileName;
            Properties.Settings.Default.Save();
            OnPropertyChanged("WindowTitle");
        }


        #region File Exists handling - need refactor

        private void OnFileExistsAskEvent(FileExistsAskEventArgs args)
        {
            // Init dialog
            FileExistsDialogViewModel fileExistsDialogViewModel = new FileExistsDialogViewModel(args.SourceFile, args.DestinationFolder);
            FileExistsDialogView fileExistsDialogView = new FileExistsDialogView();

            fileExistsDialogView.DataContext = fileExistsDialogViewModel;
            bool closing = false;

            fileExistsDialogViewModel.CloseDialog = () =>
            {
                closing = true;
                fileExistsDialogView.Close();
            };

            fileExistsDialogView.Closing += (s, e) =>
            {
                if (!closing)
                {   // Closed with X (all other closing options should go through CloseDialog).
                    fileExistsDialogViewModel.Cancel = true;
                }
            };

            Status = "File exists - waiting for user decision...";

            // #### SHOW DIALOG
            fileExistsDialogView.Owner = Application.Current.MainWindow;
            fileExistsDialogView.ShowDialog();
            // #### 

            Status = "";

            // Save the response to return
            args.Response = fileExistsDialogViewModel.Response;
            args.Cancel = fileExistsDialogViewModel.Cancel;
            args.DontAskAgain = fileExistsDialogViewModel.DontAskAgain;

            fileExistsDialogViewModel.Dispose();
        }



        #endregion

        public string WindowTitle
        {
            get
            {
                return string.Format("PicPick - {0}{1}", Path.GetFileName(ProjectLoader.FileName), CurrentProject.IsDirty ? "*" : "");
            }
        }


        public PicPickProject CurrentProject { get => ProjectLoader.Project; }


        public ActivityViewModel ActivityViewModel => _activityViewModels[CurrentActivity];

        public IActivity CurrentActivity
        {
            get => _currentActivity;
            set
            {
                if (_currentActivity != value)
                {
                    _currentActivity = value;
                    if (_currentActivity == null)
                        return;
                    lock (_currentActivity)
                    {
                        if (!_activityViewModels.ContainsKey(_currentActivity))
                        {
                            _activityViewModels.Add(_currentActivity, new ActivityViewModel(_currentActivity, new ProgressInformation()));
                            _currentActivity.StateMachine.PropertyChanged += (s, e) => OnPropertyChanged(nameof(DebugInfo));
                        }
                    }
                    var activityViewModel = _activityViewModels[_currentActivity];
                    StartCommand = activityViewModel.ExecutionViewModel.StartCommand;
                    StopCommand = activityViewModel.ExecutionViewModel.StopCommand;
                    AnalyzeCommand = activityViewModel.ExecutionViewModel.ShowPlanCommand;
                }
                OnPropertyChanged("CurrentActivity");
                OnPropertyChanged("ActivityViewModel");
            }
        }

        public Dictionary<FileExistsResponseEnum, FileExistsResponseAttribute> FileExistsResponseList => FileExistsResponse.Dictionary;

        public FileExistsResponseEnum SelectedFileExistsResponse
        {
            get { return CurrentProject.Options.FileExistsResponse; }
            set
            {
                CurrentProject.Options.FileExistsResponse = value;
                OnPropertyChanged(nameof(SelectedFileExistsResponse));
            }
        }

        public string VersionString => $"v{AppInfo.AppVersionString}";

        public string LogFile { get; set; }

        System.Windows.Media.Brush _isRunningColor = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#FF9DDA3E");
        private string status;
        private string _debugInfo;

        public System.Windows.Media.Brush RunningColor
        {
            get => ActivityViewModel.IsRunning ? _isRunningColor : null;
        }

        public Properties.GeneralUserSettings GeneralUserSettings
        {
            get { return Properties.UserSettings.General; }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string DebugInfo
        {
            get {

#if DEBUG
                return CurrentActivity?.StateMachine.CurrentState.ToString();
#else
                return "";
#endif
            }
            set
            {
                _debugInfo = value;
                OnPropertyChanged(nameof(DebugInfo));
            }
        }

        public void Dispose()
        {
            GeneralUserSettings.Save();

            ApplicationService.Instance.EventAggregator.GetEvent<GotDirtyEvent>().Unsubscribe(OnGotDirty);
            ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Unsubscribe(OnFileExistsAskEvent);
            ApplicationService.Instance.EventAggregator.GetEvent<FileErrorEvent>().Unsubscribe(OnFileErrorEvent);

            _currentActivity = null;
            _activityViewModels = null;
        }


    }
}
