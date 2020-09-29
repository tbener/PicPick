using PicPick.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public class StateTransition_Run : BaseStateTransition
    {
        Runner runner;

        public StateTransition_Run(StateManager manager) : base(manager)
        {
            runner = (Runner)manager.CoreActions.Runner;
        }

        protected override async Task Action()
        {
            Debug.WriteLine("Performing file copying...");
            await runner.RunAsync(ProgressInfo);
        }
    }
}
