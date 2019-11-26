using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PicPick.Commands;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using TalUtils;
using PicPick.ViewModel.Dialogs;
using PicPick.View.Dialogs;

namespace PicPick.ViewModel.UserControls
{
    public class ActivityViewModel : BaseViewModel
    {
        #region Private Members

        //CancellationTokenSource cts;

        PicPickProjectActivity _activity;
        string _sourceFilesStatus;
        CancellationTokenSource ctsSourceCheck;

        #endregion

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand AddDestinationCommand { get; set; }

        #endregion


        public ActivityViewModel(PicPickProjectActivity activity)
        {

            AddDestinationCommand = new RelayCommand(AddDestination);
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);

            Activity = activity;

            ProgressInfo = new ProgressInformation();
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

        private bool WarningsBeforeStart()
        {
            if (!_activity.DeleteSourceFiles)
                return true;

            if (!Properties.UserSettings.General.WarnDeleteSource)
                return true;

            string msgText = "The files will be deleted from the source folder if the operation will end successfully.\nDo you want to continue?";
            MessageBoxResult result = MessageBoxHelper.Show(msgText, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, out bool dontShowAgain);

            Properties.UserSettings.General.WarnDeleteSource = !dontShowAgain;

            return result == MessageBoxResult.Yes;
        }

        private void DisplayEndSummary()
        {
            int fileCount = Activity.FileMapping.SourceFiles.Values.Where(f => f.Status != FILE_STATUS.NONE).Count();
            string text = $"{fileCount} files processed";

            MessageBoxHelper.Show(text, "Done");
        }

        public async void Start()
        {
            ProgressInfo.Reset();

            if (!WarningsBeforeStart())
                return;

            try
            {
                ProgressInfo.RenewToken();

                await Activity.FileMapping.Compute(ProgressInfo);
                await Activity.Runner.Run(ProgressInfo);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(ex);
            }
        }

        public async void Start11()
        {
            if (!WarningsBeforeStart())
                return;

            try
            {
                //cts = new CancellationTokenSource();

                Runner runner = new Runner(Activity, ProjectLoader.Project.Options);
                //await runner.Run(ProgressInfo, cts.Token);

            }
            catch (OperationCanceledException)
            {
                // user cancelled...
            }
            finally
            {
                OnPropertyChanged("ProgressInfo");
                //cts.Dispose();

                DisplayEndSummary();
            }
        }

        private bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !Activity.IsRunning;
        }


        private void Stop()
        {
            ProgressInfo.Cancel();
        }

        private bool CanStop(object obj)
        {
            return Activity.IsRunning;
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
            if (Activity.DestinationList.Count == 1)
                Activity.DestinationList.First().Active = true;
        }

        private void AddDestination()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
            {
                Path = "",
                Template = "dd-yy"
            };
            Activity.DestinationList.Add(dest);
            AddDestinationViewModel(dest);
        }

        private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        {
            var vm = new DestinationViewModel(dest, Activity.Source);
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

        public string SourceFilesStatus
        {
            get => _sourceFilesStatus;
            set
            {
                _sourceFilesStatus = value;
                OnPropertyChanged("SourceFilesStatus");
            }
        }

        private bool _keepDestinationsAbsolute;

        public bool KeepDestinationsAbsolute
        {
            get { return DestinationViewModelList.FirstOrDefault().Destination.KeepAbsolute; }
            set
            {
                _keepDestinationsAbsolute = value;
                foreach (DestinationViewModel destvm in DestinationViewModelList)
                {
                    destvm.Destination.KeepAbsolute = _keepDestinationsAbsolute;
                }
            }
        }



    }
}
