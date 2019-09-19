using PicPick.Models;
using PicPick.Commands;
using PicPick.Interfaces;
using PicPick.Model;
using PicPick.UserControls.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TalUtils;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;
using PicPick.Helpers;
using System.ComponentModel;
using PicPick.Core;
using PicPick.View;

namespace PicPick.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {

        private PicPickProjectActivity _currentActivity;

        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddActivityCommand { get; set; }
        public ICommand DeleteActivityCommand { get; set; }

        public MainWindowViewModel()
        {

            OpenFileCommand = new RelayCommand(OpenFileWithDialog);
            SaveCommand = new SaveCommand();
            AddActivityCommand = new RelayCommand(AddNewActivity);
            DeleteActivityCommand = new RelayCommand(DeleteActivity, () => CurrentProject.ActivityList.Count > 1);

            ProjectLoader.OnSaveEventHandler += (s, e) => UpdateFileName();

            if (string.IsNullOrEmpty(Properties.Settings.Default.LastFile) || !File.Exists(Properties.Settings.Default.LastFile))
                ProjectLoader.LoadCreateDefault();
            else
                OpenFile(Properties.Settings.Default.LastFile);

            CurrentProject.GetIsDirtyInstance().OnGotDirty += (s, e) => OnGotDirty();

            // set the current activity
            CurrentActivity = CurrentProject.ActivityList.FirstOrDefault();
            //CurrentProject.IsDirty = isDirty;

            ApplicationService.Instance.EventAggregator.GetEvent<ActivityStartedEvent>().Subscribe(OnActivityStart);
            ApplicationService.Instance.EventAggregator.GetEvent<ActivityEndedEvent>().Subscribe(OnActivityEnd);
            ApplicationService.Instance.EventAggregator.GetEvent<GotDirtyEvent>().Subscribe(OnGotDirty);
        }

        private void OnGotDirty()
        {
            OnPropertyChanged("WindowTitle");
        }

        private void AddNewActivity()
        {
            PicPickProjectActivity act = new PicPickProjectActivity("New Activity");
            CurrentProject.ActivityList.Add(act);
            CurrentActivity = act;
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
            string file = "";
            if (DialogHelper.BrowseOpenFileByExtensions(new[] { "picpick" }, true, ref file))
            {
                OpenFile(file);
            }

        }

        private void OpenFile(string file)
        {
            if (!ProjectLoader.Load(file)) return;
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
            if (!ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Contains(OnFileExistsAskEvent))
                ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Subscribe(OnFileExistsAskEvent);
        }
        private void OnActivityEnd()
        {
            ApplicationService.Instance.EventAggregator.GetEvent<FileExistsAskEvent>().Unsubscribe(OnFileExistsAskEvent);
        }

        private void OnFileExistsAskEvent(FileExistsAskEventArgs args)
        {
            // Init dialog
            FileExistsDialogViewModel vm = new FileExistsDialogViewModel(args.SourceFile, args.DestinationFolder);
            FileExistsDialogView view = new FileExistsDialogView() { DataContext = vm };
            vm.CloseDialog = () => { view.Close(); };

            // #### SHOW DIALOG
            view.ShowDialog();
            // #### 

            // Save the response to return
            args.Response = vm.Response;
            args.Cancel = vm.Cancel;
            args.DontAskAgain = vm.DontAskAgain;
            
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


    }
}
