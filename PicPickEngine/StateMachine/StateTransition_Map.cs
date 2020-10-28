using PicPick.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public class StateTransition_Map : BaseStateTransition
    {
        Mapper mapper;

        public StateTransition_Map(StateManager manager) : base(manager)
        {
            mapper = (Mapper)manager.CoreActions.Mapper;
        }

        protected override async Task<bool> Action()
        {
            Debug.WriteLine("Running Mapping...");
            ProgressInfo.Text = "Calculating...";
            await mapper.MapAsync(ProgressInfo);
            return true;
        }
    }
}
