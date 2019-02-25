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

namespace PicPick.ViewModel
{
    class MainWindowViewModel : BaseViewModel
    {
        private PicPickProjectActivity _currentActivity;
        private PathBrowserViewModel _activitySourceViewModel;
        private ActivityViewModel _activityViewModel;

        public ICommand OpenFileCommand { get; set; }


        public MainWindowViewModel()
        {
            OpenFileCommand = new RelayCommand(OpenFile);

            CurrentProject = new PicPickProject(true)
            {
                Name = "New Project"
            };
            CurrentProject.PropertyChanged += CurrentProject_PropertyChanged;

            PicPickProjectActivity act1 = new PicPickProjectActivity("Test");
            PicPickProjectActivity act2 = new PicPickProjectActivity("Blah blah");
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

            // set the current activity
            CurrentActivity = act1;
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
