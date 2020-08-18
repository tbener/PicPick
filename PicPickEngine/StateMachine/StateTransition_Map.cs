using PicPick.Core;
using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        protected override async Task Action()
        {
            Debug.WriteLine("Running Mapping...");
            await mapper.MapAsync(ProgressInfo);
        }
    }
}
