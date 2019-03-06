using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PicPick.Commands;
using PicPick.Project;
using PicPick.UserControls.ViewModel;

namespace PicPick.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        private PicPickProjectActivity _activity;
        public ICommand AddDestinationCommand { get; set; }

        public ActivityViewModel(PicPickProjectActivity activity)
        {
            AddDestinationCommand = new RelayCommand(AddDestination);

            Activity = activity;
        }

        /// <summary>
        /// Associate the ViewModels to the inner objects
        /// </summary>
        private void InitActivity()
        {
            // source
            SourceViewModel = new PathBrowserViewModel(Activity.Source);

            // destinations
            DestinationViewModelList = new ObservableCollection<DestinationViewModel>();
            foreach (PicPickProjectActivityDestination dest in Activity.DestinationList)
                AddDestinationViewModel(dest);

            ExecutionViewModel = new ExecutionViewModel(Activity);
        }

        private void OnDestinationDelete(object sender, EventArgs e)
        {
            DestinationViewModel vm = sender as DestinationViewModel;
            if (vm == null) return;
            if (vm.Destination != null)
                Activity.DestinationList.Remove(vm.Destination);
            DestinationViewModelList.Remove(vm);
        }

        private void AddDestination()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
            {
                Path = "",
                Template = "dd-YY"
            };
            Activity.DestinationList.Add(dest);
            AddDestinationViewModel(dest);
        }

        private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        {
            var vm = new DestinationViewModel(dest);
            vm.OnDeleteClicked += OnDestinationDelete;
            DestinationViewModelList.Add(vm);
        }

        public PicPickProjectActivity Activity
        {
            get => _activity; set
            {
                _activity = value;
                InitActivity();
                OnPropertyChanged("Activity");
            }
        }

        public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }



        public PathBrowserViewModel SourceViewModel
        {
            get { return (PathBrowserViewModel)GetValue(SourceViewModelProperty); }
            set { SetValue(SourceViewModelProperty, value); }
        }

        public static readonly DependencyProperty SourceViewModelProperty =
            DependencyProperty.Register("SourceViewModel", typeof(PathBrowserViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));




        public ExecutionViewModel ExecutionViewModel
        {
            get { return (ExecutionViewModel)GetValue(ExecutionViewModelProperty); }
            set { SetValue(ExecutionViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExecutionViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExecutionViewModelProperty =
            DependencyProperty.Register("ExecutionViewModel", typeof(ExecutionViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));



    }
}
