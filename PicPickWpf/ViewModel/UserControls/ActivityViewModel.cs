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
using System.IO;
using PicPick.ViewModel.UserControls.Mapping;

namespace PicPick.ViewModel.UserControls
{
    public class ActivityViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        //CancellationTokenSource cts;

        //PicPickProjectActivity _activity;
        //string _sourceFilesStatus;
        //CancellationTokenSource ctsSourceCheck;

        private bool _isRunning;

        
        #endregion

        #region Commands

        public ICommand StartCommand { get; set; }
        public ICommand AnalyzeCommand { get; set; }
        public ICommand StopCommand { get; set; }
        //public ICommand AddDestinationCommand { get; set; }

        #endregion

        #region CTOR

        public ActivityViewModel(PicPickProjectActivity activity)
        {

            //AddDestinationCommand = new RelayCommand(AddDestination);
            StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
            AnalyzeCommand = new AsyncRelayCommand(AnalyzeAsync, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);

            Activity = activity;
            InitActivity();

            ProgressInfo = new ProgressInformation();
            //((Progress<ProgressInformation>)ProgressInfo.Progress).ProgressChanged += ActivityViewModel_ProgressChanged;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Associate the ViewModels to the inner objects
        /// </summary>
        private void InitActivity()
        {
            
            SourceViewModel = new SourceViewModel(Activity);
            DestinationListViewModel = new DestinationListViewModel(Activity);

            Activity.OnActivityStateChanged += Activity_OnActivityStateChanged;

        }



       


        #endregion

        #region Execution

        private bool WarningsBeforeStart()
        {
            if (!Activity.DeleteSourceFiles)
                return true;

            if (!Properties.UserSettings.General.WarnDeleteSource)
                return true;

            string msgText = "The files will be deleted from the source folder if the operation will end successfully.\nDo you want to continue?";
            MessageBoxResult result = MessageBoxHelper.Show(msgText, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, out bool dontShowAgain);

            Properties.UserSettings.General.WarnDeleteSource = !dontShowAgain;

            return result == MessageBoxResult.Yes;
        }


        private async void Activity_OnActivityStateChanged(PicPickProjectActivity activity, ActivityStateChangedEventArgs e)
        {
            MappingBaseViewModel vm;
            bool dontShowAgain;

            switch (activity.State)
            {
                case ACTIVITY_STATE.NOT_STARTED:
                    break;
                case ACTIVITY_STATE.ANALYZING:
                    break;
                case ACTIVITY_STATE.ANALYZED:
                    if (!Properties.UserSettings.General.ShowPreviewWindow && !Activity.RunMode_AnalyzeOnly)
                        return;
                    vm = new MappingPlanViewModel(Activity);
                    var result = MessageBoxHelper.Show(vm, "Mapping Preview", MessageBoxButton.OKCancel, out dontShowAgain);
                    if (dontShowAgain)
                        Properties.UserSettings.General.ShowPreviewWindow = false;
                    if (result != MessageBoxResult.OK)
                        e.Cancel = true;
                    break;
                case ACTIVITY_STATE.RUNNING:
                    break;
                case ACTIVITY_STATE.DONE:
                    if (!Properties.UserSettings.General.ShowSummaryWindow)
                        return;
                    vm = new MappingResultsViewModel(Activity);
                    MessageBoxHelper.Show(vm, "Finished", MessageBoxButton.OK, out dontShowAgain);
                    if (dontShowAgain)
                        Properties.UserSettings.General.ShowSummaryWindow = false;
                    break;
                default:
                    break;
            }
        }

        public async Task AnalyzeAsync()
        {
            Activity.RunMode_AnalyzeOnly = true;
            await StartAsync();
            Activity.RunMode_AnalyzeOnly = false;
        }

        public async Task StartAsync()
        {
            ProgressInfo.Reset();
            ProgressInfo.Report();

            _log.Info("Start");

            if (!WarningsBeforeStart())
                return;

            try
            {
                ProgressInfo.RenewToken();
                IsRunning = true;
                EventAggregatorHelper.PublishActivityStarted();

                await Activity.ExecuteAsync(ProgressInfo);

            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex);
                MessageBoxHelper.Show(ex);
            }
            finally
            {
                IsRunning = false;
                await Task.Run(() => ProgressInfo.Finished());
                CommandManager.InvalidateRequerySuggested();
            }

        }


        public bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path) && !IsRunning;
        }


        private void Stop()
        {
            ProgressInfo.Cancel();
        }

        private bool CanStop()
        {
            return IsRunning;
        }



        public ProgressInformation ProgressInfo { get; set; }

        #endregion


        //private void OnDestinationDelete(object sender, EventArgs e)
        //{
        //    DestinationViewModel vm = sender as DestinationViewModel;
        //    if (vm == null) return;
        //    if (vm.Destination != null)
        //        Activity.DestinationList.Remove(vm.Destination);
        //    DestinationViewModelList.Remove(vm);
        //    if (Activity.DestinationList.Count == 1)
        //        Activity.DestinationList.First().Active = true;
        //}

        //private void AddDestination()
        //{
        //    PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination()
        //    {
        //        Path = "",
        //        Template = "dd-yy"
        //    };
        //    Activity.DestinationList.Add(dest);
        //    AddDestinationViewModel(dest);
        //}

        //private void AddDestinationViewModel(PicPickProjectActivityDestination dest)
        //{
        //    var vm = new DestinationViewModel(dest, Activity.Source);
        //    vm.OnDeleteClicked += OnDestinationDelete;
        //    DestinationViewModelList.Add(vm);
        //}

        public void Dispose()
        {
            Activity = null;
        }


        public PicPickProjectActivity Activity { get; set; }

        //public ObservableCollection<DestinationViewModel> DestinationViewModelList { get; set; }




        public SourceViewModel SourceViewModel
        {
            get { return (SourceViewModel)GetValue(SourceViewModelProperty); }
            set { SetValue(SourceViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SourceViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceViewModelProperty =
            DependencyProperty.Register("SourceViewModel", typeof(SourceViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));


        public DestinationListViewModel DestinationListViewModel
        {
            get { return (DestinationListViewModel)GetValue(DestinationListViewModelProperty); }
            set { SetValue(DestinationListViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DestinationListViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DestinationListViewModelProperty =
            DependencyProperty.Register("DestinationListViewModel", typeof(DestinationListViewModel), typeof(ActivityViewModel), new PropertyMetadata(null));





        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

    }
}
