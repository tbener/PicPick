using PicPick.Models.Interfaces;
using System;
using System.Windows.Input;

namespace PicPick.Commands
{
    public class StopCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        IActivity _activity;

        public StopCommand(IActivity activity)
        {
            _activity = activity;
            _activity.OnActivityStateChanged += (s, e) => CanExecuteChanged?.Invoke(s, e);
        }

        public bool CanExecute(object parameter)
        {
            return _activity.IsRunning;
        }


        public void Execute(object parameter)
        {
            _activity.IsRunning = false;
            _activity.StateMachine.Stop();
        }

    }
}
