using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TalUtils;

namespace PicPick.Commands
{
    public class StartCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        IActivity _activity;

        public StartCommand(IActivity activity)
        {
            _activity = activity;
            _activity.OnActivityStateChanged += (s, e) => CanExecuteChanged?.Invoke(s, e);
        }

        public bool CanExecute(object parameter)
        {
            return !_activity.IsRunning;
        }

        
        public void Execute(object parameter)
        {
            if (Properties.UserSettings.General.WarnDeleteSource)
                if (WarningsBeforeStart(_activity))
                {
                    _activity.IsRunning = true;
                    _activity.StateMachine.Start(PicPickState.DONE);
                }
        }

        #region Helper methods

        private bool WarningsBeforeStart(IActivity activity)
        {
            if (!activity.DeleteSourceFiles)
                return true;

            if (!Properties.UserSettings.General.WarnDeleteSource)
                return true;

            string msgText = "The files will be deleted from the source folder if the operation will end successfully.\nDo you want to continue?";
            MessageBoxResult result = MessageBoxHelper.Show(msgText, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, out bool dontShowAgain);

            Properties.UserSettings.General.WarnDeleteSource = !dontShowAgain;

            return result == MessageBoxResult.Yes;
        }

        #endregion
    }
}
