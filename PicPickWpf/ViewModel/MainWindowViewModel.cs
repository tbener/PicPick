using PicPick.Project;
using PicPickUI.Commands;
using PicPickUI.Interfaces;
using PicPickUI.Model;
using PicPickUI.UserControls.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TalUtils;

namespace PicPickUI.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private PicPickProjectActivity _currentActivity;
        private PathBrowserViewModel _activitySourceViewModel;

        public ICommand OpenFileCommand { get; set; }


        public MainWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile);

            CurrentProject = new PicPickProject(true)
            {
                Name = "New Project"
            };
            CurrentProject.PropertyChanged += CurrentProject_PropertyChanged;

            PicPickProjectActivity act1 = new PicPickProjectActivity() { Name = "Test" };
            PicPickProjectActivity act2 = new PicPickProjectActivity() { Name = "Blah blah" };
            CurrentProject.ActivityList.Add(act1);
            CurrentProject.ActivityList.Add(act2);

            act1.Source = new PicPickProjectActivitySource()
            {
                Path = "C:\\temp",
                Filter = "*.jpg"
            };

            PicPickProjectActivityDestination dest1 = new PicPickProjectActivityDestination()
            {
                Path = "Folder1",
                Template = "YYYY-mm"
            };
            PicPickProjectActivityDestination dest2 = new PicPickProjectActivityDestination()
            {
                Path = "Folder2",
                Template = "dd-YY"
            };
            act1.DestinationList.AddRange(new[] { dest1, dest2 });
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsDirty") || e.PropertyName.Equals("Name"))
                OnPropertyChanged("WindowTitle");
        }

        void OpenFile()
        {
            Msg.Show("hello");
        }


        public string WindowTitle
        {
            get { return string.Format("{0}{1} - PicPick", CurrentProject.Name, CurrentProject.IsDirty ? "*" : ""); }
        }

        public PicPickProject CurrentProject { get; set; }
        public PicPickProjectActivity CurrentActivity
        {
            get => _currentActivity;
            set
            {
                if (_currentActivity != value)
                {
                    _currentActivity = value;
                    ActivitySourceViewModel = new PathBrowserViewModel(_currentActivity.Source);
                }
                OnPropertyChanged("CurrentActivity");
            }
        }

        public PathBrowserViewModel ActivitySourceViewModel
        {
            get => _activitySourceViewModel;
            set
            {
                _activitySourceViewModel = value;
                OnPropertyChanged("ActivitySourceViewModel");
            }
        }
    }
}
