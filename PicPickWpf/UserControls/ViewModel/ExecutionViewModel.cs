using PicPick.Commands;
using PicPick.Models;
using PicPick.Project;
using PicPick.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{

    public class ExecutionViewModel : BaseViewModel
    {
        #region Private Members

        ProgressInformation progressInfo;
        CancellationTokenSource cts;

        #region Commands

        public ICommand AnalyzeCommand { get; set; }
        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }

        #endregion

        #endregion

        #region CTOR

        public ExecutionViewModel(PicPickProjectActivity activity)
        {
            ProgressInfo = new ProgressInformation();
            ((Progress<ProgressInformation>)ProgressInfo.Progress).ProgressChanged += ProgressInformation_ProgressChanged;
            cts = new CancellationTokenSource();
            Activity = activity;

            AnalyzeCommand = new RelayCommand(Analyze);
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
        }
        
        #endregion

        public async void Start()
        {
            try
            {
                await Activity.Start(progressInfo, cts.Token);

                OnPropertyChanged("ProgressInfo");
            }
            catch (OperationCanceledException)
            {
                // user cancelled...
            }
        }

        public async void Analyze()
        {
            try
            {
                await Activity.Analyze(progressInfo, cts.Token);

                OnPropertyChanged("ProgressInfo");
            }
            catch (OperationCanceledException)
            {
                // user cancelled...
            }
        }

        private bool CanStart()
        {
            return !string.IsNullOrEmpty(Activity.Source.Path);
        }


        private void Stop()
        {
            throw new NotImplementedException();
        }

        private bool CanStop()
        {
            return true;
        }

        private void PrepareProgressInfo()
        {

        }

        private void ProgressInformation_ProgressChanged(object sender, ProgressInformation e)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            //throw new NotImplementedException();
        }

        #region Public Properties

        PicPickProjectActivity Activity { get; set; }

        public int ProgressValue { get; set; }
        public ProgressInformation ProgressInfo { get => progressInfo; set => progressInfo = value; }

        #endregion
    }
}
