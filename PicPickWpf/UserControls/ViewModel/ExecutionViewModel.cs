using PicPick.Commands;
using PicPick.Models;
using PicPick.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PicPick.UserControls.ViewModel
{

    public class ExecutionViewModel
    {
        #region Private Members

        ProgressInformation progressInfo;
        CancellationTokenSource cts;

        #region Commands

        ICommand StartCommand { get; set; }
        ICommand StopCommand { get; set; }

        #endregion

        #endregion

        #region CTOR

        public ExecutionViewModel(PicPickProjectActivity activity)
        {
            progressInfo = new ProgressInformation();
            ((Progress<ProgressInformation>)progressInfo.Progress).ProgressChanged += ProgressInformation_ProgressChanged;
            cts = new CancellationTokenSource();
            Activity = activity;

            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
        }
        
        #endregion

        public async void Start()
        {
            try
            {
                await Activity.Analyze(cts.Token);
                await Activity.Start(progressInfo, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // user cancelled...
            }
        }

        private bool CanStart()
        {
            return true;
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
            throw new NotImplementedException();
        }

        #region Public Properties

        PicPickProjectActivity Activity { get; set; }

        public int ProgressValue { get; set; }

        #endregion
    }
}
