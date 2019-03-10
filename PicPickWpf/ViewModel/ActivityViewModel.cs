using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PicPick.Commands;
using PicPick.Project;
using PicPick.UserControls.ViewModel;
using TalUtils;

namespace PicPick.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        private PicPickProjectActivity _activity;
        private string _sourceFilesStatus;
        CancellationTokenSource ctsSourceCheck;

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
            Activity.Source.PropertyChanged += Source_PropertyChanged;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CheckSourceStatus();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // destinations
            DestinationViewModelList = new ObservableCollection<DestinationViewModel>();
            foreach (PicPickProjectActivityDestination dest in Activity.DestinationList)
                AddDestinationViewModel(dest);

            ExecutionViewModel = new ExecutionViewModel(Activity);
        }

        private async Task CheckSourceStatus()
        {
            

            try
            {
                // Cancel previous operations
                ctsSourceCheck?.Cancel();

                // reset state
                SourceFilesStatus = "";

                if (!PathHelper.Exists(Activity.Source.Path))
                    return;

                // Create a new cancellations token and await a new task to count files
                ctsSourceCheck = new CancellationTokenSource();
                int count = await Task.Run(() => Activity.Source.FileList.Count);
                SourceFilesStatus = $"{count} files found";
            }
            catch (OperationCanceledException)
            {
                // operation was canceled
            }
            catch (Exception)
            {
                // error in counting files. most probably because folder doesn't exist.
                SourceFilesStatus = "---";
            }
        }

        private async void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await CheckSourceStatus();
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



        public string SourceFilesStatus
        {
            get => _sourceFilesStatus;
            set
            {
                _sourceFilesStatus = value;
                OnPropertyChanged("SourceFilesStatus");
            }
        }

        

    }
}
