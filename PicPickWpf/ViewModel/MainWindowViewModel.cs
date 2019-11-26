﻿using PicPick.Models;
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
using PicPick.View;
using log4net;
using PicPick.ViewModel.Dialogs;
using PicPick.View.Dialogs;
using PicPick.ViewModel.UserControls;

namespace PicPick.ViewModel
{
    class MainWindowViewModel : BaseViewModel, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private PicPickProjectActivity _currentActivity;

        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddActivityCommand { get; set; }
        public ICommand DeleteActivityCommand { get; set; }
        public ICommand BrowseLogFileCommand { get; set; }
        public ICommand SendMailDialogCommand { get; set; }
        public ICommand OpenPageCommand { get; set; }

        public MainWindowViewModel()
        {

            OpenFileCommand = new RelayCommand(OpenFileWithDialog);
            SaveCommand = new SaveCommand();
            AddActivityCommand = new RelayCommand(CreateNewActivity);
            DeleteActivityCommand = new RelayCommand(DeleteActivity, () => CurrentProject.ActivityList.Count > 1);
            BrowseLogFileCommand = new RelayCommand(BrowseLogFile);
            SendMailDialogCommand = new RelayCommand(SendMailDialog);
            OpenPageCommand = new RelayCommand(OpenPicPickPage);

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

            CurrentProject.GetIsDirtyInstance().OnGotDirty += (s, e) => OnGotDirty();
            CurrentProject.IsDirty = false;

            CurrentActivity = CurrentProject.ActivityList.FirstOrDefault();


            ApplicationService.Instance.EventAggregator.GetEvent<ActivityStartedEvent>().Subscribe(OnActivityStart);
            ApplicationService.Instance.EventAggregator.GetEvent<ActivityEndedEvent>().Subscribe(OnActivityEnd);
            ApplicationService.Instance.EventAggregator.GetEvent<GotDirtyEvent>().Subscribe(OnGotDirty);
            ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Subscribe(OnFileExistsAskEvent);
            ApplicationService.Instance.EventAggregator.GetEvent<FileErrorEvent>().Subscribe(OnFileErrorEvent);

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

        private PicPickProjectActivity GetNewActivity(string name)
        {
            PicPickProjectActivity activity = new PicPickProjectActivity(name);
            activity.Source.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            PicPickProjectActivityDestination destination = new PicPickProjectActivityDestination();
            destination.Path = "SampleDestination";
            activity.DestinationList.Add(destination);
            return activity;
        }

        private PicPickProject CreateNewProject()
        {
            PicPickProject proj = new PicPickProject()
            {
                Name = "Default"
            };

            string activityName = (string)Application.Current.FindResource("ppk_first_activity");
            PicPickProjectActivity activity = GetNewActivity(activityName);
            proj.ActivityList.Add(activity);

            return proj;
        }

        private void CreateNewActivity()
        {
            string activityName = (string)Application.Current.FindResource("ppk_new_activity");
            PicPickProjectActivity activity = GetNewActivity(activityName);
            CurrentProject.ActivityList.Add(activity);
            CurrentActivity = activity;
        }

        private void LoadOrCreateNew()
        {
            if (!ProjectLoader.LoadDefault())
            {
                PicPickProject project = CreateNewProject();
                ProjectLoader.Create(project);
            }
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
                int selIndex = CurrentProject.ActivityList.IndexOf(CurrentActivity);
                CurrentProject.ActivityList.Remove(CurrentActivity);

                // set selected activity
                if (CurrentProject.ActivityList.Count <= selIndex)
                    selIndex--;
                CurrentActivity = CurrentProject.ActivityList[selIndex];
            }
        }



        void OpenFileWithDialog()
        {
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
        }

        private void UpdateFileName()
        {
            OnPropertyChanged("CurrentProject");
            Properties.Settings.Default.LastFile = ProjectLoader.FileName;
            Properties.Settings.Default.Save();
            OnPropertyChanged("WindowTitle");
        }


        #region File Exists handling - need refactor

        private void OnActivityStart()
        {
            Status = "Running...";
            //if (!ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Contains(OnFileExistsAskEvent))
            //    ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Subscribe(OnFileExistsAskEvent);
        }
        private void OnActivityEnd()
        {
            Status = "Ready";
        }


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

            Status = "File exists - displaying dialog";

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


        public ActivityViewModel ActivityViewModel
        {
            get { return (ActivityViewModel)GetValue(ActivityViewModelProperty); }
            set { SetValue(ActivityViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActivityViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActivityViewModelProperty =
            DependencyProperty.Register("ActivityViewModel", typeof(ActivityViewModel), typeof(MainWindowViewModel), new PropertyMetadata(null));



        public PicPickProjectActivity CurrentActivity
        {
            get => _currentActivity;
            set
            {
                if (_currentActivity != value)
                {
                    //FileExistsResponseEnum fer = _currentActivity == null ?
                    //    (FileExistsResponseEnum)Enum.Parse(typeof(FileExistsResponseEnum), Properties.Settings.Default.FileExistsResponse, true)
                    //    : _currentActivity.FileExistsResponse;
                    _currentActivity = value;
                    //_currentActivity.FileExistsResponse = fer;
                    ActivityViewModel = new ActivityViewModel(_currentActivity);
                }
                OnPropertyChanged("CurrentActivity");
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

        public System.Windows.Media.Brush RunningColor
        {
            get => CurrentActivity.IsRunning ? _isRunningColor : null;
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

        public void Dispose()
        {
            ApplicationService.Instance.EventAggregator.GetEvent<ActivityStartedEvent>().Unsubscribe(OnActivityStart);
            ApplicationService.Instance.EventAggregator.GetEvent<ActivityEndedEvent>().Unsubscribe(OnActivityEnd);
            ApplicationService.Instance.EventAggregator.GetEvent<GotDirtyEvent>().Unsubscribe(OnGotDirty);
            ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Unsubscribe(OnFileExistsAskEvent);
            ApplicationService.Instance.EventAggregator.GetEvent<FileErrorEvent>().Unsubscribe(OnFileErrorEvent);

            _currentActivity = null;
        }


    }
}
