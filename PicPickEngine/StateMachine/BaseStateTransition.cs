using PicPick.Core;
using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public abstract class BaseStateTransition : IStateHandler
    {
        protected StateManager _stateManager;
        protected IProgressInformation ProgressInfo => _stateManager.ProgressInfo;

        public bool IsRunning { get; private set; }

        public BaseStateTransition(StateManager manager)
        {
            _stateManager = manager;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                IsRunning = true;
                await Action();
                ProgressInfo.Text = "";
            }
            catch (Exception ex)
            {
                // log
                throw ex;
            }
            finally
            {
                IsRunning = false;
            }
        }

        protected abstract Task Action();
    }
}
