using PicPick.Project;
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

namespace PicPick.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        const FILE_EXISTS_RESPONSE DEFAULT_FILE_EXISTS_RESPONSE = FILE_EXISTS_RESPONSE.ASK;

        private PicPickProjectActivity _currentActivity;
        private Dictionary<FILE_EXISTS_RESPONSE, KeyValuePair<string, string>> _fileExistsResponses;

        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddActivityCommand { get; set; }
        public ICommand DeleteActivityCommand { get; set; }

        public MainWindowViewModel()
        {

            OpenFileCommand = new RelayCommand(OpenFile);
            SaveCommand = new SaveCommand();
            AddActivityCommand = new RelayCommand(AddNewActivity);
            DeleteActivityCommand = new RelayCommand(DeleteActivity, () => CurrentProject.ActivityList.Count > 1);

            ProjectLoader.SupportIsDirty = true;
            ProjectLoader.OnSaveEventHandler += (s, e) => UpdateFileName();

            if (string.IsNullOrEmpty(Properties.Settings.Default.LastFile))
                ProjectLoader.LoadCreateDefault();
            else
                OpenFile(Properties.Settings.Default.LastFile);

            CurrentProject.PropertyChanged += CurrentProject_PropertyChanged;

            // set the current activity
            CurrentActivity = CurrentProject.ActivityList.FirstOrDefault();
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



        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsDirty") || e.PropertyName.Equals("Name"))
                OnPropertyChanged("WindowTitle");
        }

        void OpenFile()
        {
            string file = "";
            if (DialogHelper.BrowseOpenFileByExtensions(new[] { "picpick" }, true, ref file))
            {
                OpenFile(file);
            }

        }

        private void UpdateFileName()
        {
            OnPropertyChanged("CurrentProject");
            Properties.Settings.Default.LastFile = ProjectLoader.FileName;
            Properties.Settings.Default.Save();
            OnPropertyChanged("WindowTitle");
        }

        private void OpenFile(string file)
        {
            if (!ProjectLoader.Load(file)) return;
            UpdateFileName();
        }




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
                    FILE_EXISTS_RESPONSE fer = _currentActivity == null ? DEFAULT_FILE_EXISTS_RESPONSE : _currentActivity.FileExistsResponse;
                    _currentActivity = value;
                    _currentActivity.FileExistsResponse = fer;
                    ActivityViewModel = new ActivityViewModel(_currentActivity);
                }
                OnPropertyChanged("CurrentActivity");
            }
        }

        public Dictionary<FILE_EXISTS_RESPONSE, string> FileExistsResponseList => Model.FileExistsResponse.GetDictionary;

        public FILE_EXISTS_RESPONSE SelectedFileExistsResponse
        {
            get { return CurrentActivity.FileExistsResponse; }
            set
            {
                CurrentActivity.FileExistsResponse = value;
                OnPropertyChanged(nameof(SelectedFileExistsResponse));
            }
        }


    }
}
