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
using System.Diagnostics;
using PicPick.StateMachine;
using PicPick.Models.Interfaces;

namespace PicPick.ViewModel.UserControls
{
    public class ActivityBaseViewModel : BaseViewModel, IDisposable
    {
        #region Private Members

        protected const PicPickState BACKGROUND_END_STATE = PicPickState.READY_TO_RUN;

        #endregion

        #region Public\Protected Properties

        protected PicPickProjectActivity Activity { get; private set; }
        public ProgressInformation ProgressInfo { get; private set; }

        #endregion

        #region Commands


        #endregion

        #region CTOR

        public ActivityBaseViewModel(IActivity activity, IProgressInformation progressInfo)
        {
            Activity = (PicPickProjectActivity)activity;
            Activity.OnActivityStateChanged += (s, e) => OnPropertyChanged(nameof(IsRunning));
            ProgressInfo = (ProgressInformation)progressInfo;
        }

        #endregion

        public bool IsRunning
        {
            get => Activity.State == ActivityState.RUNNING;
        }

        public virtual void Dispose()
        {
            Activity = null;
            ProgressInfo = null;
        }

        protected void UpdateMapping(PicPickState fromState)
        {
            if (BackgroundReadingEnabled)
                Activity.StateMachine.Restart(fromState, BACKGROUND_END_STATE);
            else
            {
                Activity.StateMachine.Stop();
                Activity.StateMachine.SetNeedRestart(fromState);
            }
        }

        #region Public Properties

        public virtual bool BackgroundReadingEnabled
        {
            get => Properties.UserSettings.General.BackgroundReading;
            set => Properties.UserSettings.General.BackgroundReading = value;
        }
        #endregion
    }
}
