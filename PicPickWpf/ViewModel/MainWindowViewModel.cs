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

namespace PicPick.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private PicPickProjectActivity _currentActivity;
        private PathBrowserViewModel _activitySourceViewModel;
        private ActivityViewModel _activityViewModel;

        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand AddActivityCommand { get; set; }

        public MainWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            AddActivityCommand = new RelayCommand(AddNewActivity);

            ProjectHelper.SupportIsDirty = true;

            if (string.IsNullOrEmpty(Properties.Settings.Default.LastFile))
                ProjectHelper.LoadCreateDefault();
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
            Properties.Settings.Default.LastFile = ProjectHelper.FileName;
            Properties.Settings.Default.Save();
            OnPropertyChanged("WindowTitle");
        }

        private void OpenFile(string file)
        {
            if (!ProjectHelper.Load(file)) return;
            UpdateFileName();
        }

        private void Save(string file)
        {
            if (!ProjectHelper.Save(file)) return;
            UpdateFileName();
        }

        private void Save()
        {
            if (ProjectHelper.FileName == null)
                SaveAs();
            else
                ProjectHelper.Save();
            
        }

        private void SaveAs()
        {
            string file = "";
            if (DialogHelper.BrowseSaveFileByExtensions(new[] { "picpick" }, true, ref file))
            {
                Save(file);
            }
        }

        public string WindowTitle
        {
            get {
                return string.Format("PicPick - {0}{1}", Path.GetFileName(ProjectHelper.FileName), CurrentProject.IsDirty ? "*" : "");
            }
        }


        public PicPickProject CurrentProject { get => ProjectHelper.Project; }
        

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
                    _currentActivity = value;
                    ActivityViewModel = new ActivityViewModel(_currentActivity);
                }
                OnPropertyChanged("CurrentActivity");
            }
        }

        
    }
}
