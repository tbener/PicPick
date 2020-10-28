using PicPick.Helpers;
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

        public async Task<bool> ExecuteAsync()
        {
            bool result;
            try
            {
                IsRunning = true;
                result = await Action();
                ProgressInfo.Text = "";
            }
            finally
            {
                IsRunning = false;
            }
            return result;
        }

        protected abstract Task<bool> Action();
    }
}
