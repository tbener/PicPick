using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public class StateTransition_Run : BaseStateTransition
    {
        public StateTransition_Run(StateManager manager) : base(manager)
        {

        }

        protected override async Task Action()
        {
            Debug.WriteLine("Performing file copying...");
            //_stateManager.Activity.FileMapping.MapAsync(_stateManager.Activity.)
        }
    }
}
