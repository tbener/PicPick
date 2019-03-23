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
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Project;
using PicPick.UserControls.ViewModel;
using PicPick.View;
using TalUtils;

namespace PicPick.ViewModel
{
    public class ActivityViewModel : BaseViewModel
    {
        #region Private Members

        CancellationTokenSource cts;
        bool _canExecute;

        PicPickProjectActivity _activity;
        string _sourceFilesStatus;
        CancellationTokenSource ctsSourceCheck;

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand AddDestinationCommand { get; set; }

        #endregion

        #endregion

        public ActivityViewModel(PicPickProjectActivity activity)
        {
            
            AddDestinationCommand = new RelayCommand(AddDestination);
            StartCommand = new RelayCommand(Start, CanStart);

            Activity = activity;

            ProgressInfo = new ProgressInformation();
            cts = new CancellationTokenSource();
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

            ApplicationService.Instance.EventAggregator.GetEvent<AskEvent>().Subscribe(OnFileExistsAsk);
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

        #region Execution

        public async void Start()
        {
            try
            {
                ProgressWindowViewModel progressWindowViewModel = new ProgressWindowViewModel(ProgressInfo);
                ProgressWindowView progressWindow = new ProgressWindowView()
                {
                    DataContext = progressWindowViewModel
                };
                progressWindow.Show();

                await Activity.Start(ProgressInfo, cts.Token);

                progressWindow.Close();

                OnPropertyChanged("ProgressInfo");
            }
            catch (OperationCanceledException)
            {
                // user cancelled...
            }
        }

        private bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !Activity.IsRunning;
        }

        private void OnFileExistsAsk(AskEventArgs args)
        {
            if (Msg.ShowQ("File Exists, overwrite? \nif you anwer Cancel, the file will be skipped. Same anwer will apply to all existing files."))
                args.Response = FILE_EXISTS_RESPONSE.OVERWRITE;
            else
                args.Response = FILE_EXISTS_RESPONSE.SKIP;
            args.DontAskAgain = true;
        }

        public ProgressInformation ProgressInfo { get; set; }

        #endregion

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
