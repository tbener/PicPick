using PicPick.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public class StateTransition_Filter : BaseStateTransition
    {
        Mapper mapper;

        public StateTransition_Filter(StateManager manager) : base(manager)
        {
            mapper = (Mapper)manager.CoreActions.Mapper;
        }

        protected override async Task Action()
        {
            Debug.WriteLine("Running Filtering...");
            ProgressInfo.Text = "Filtering...";
            mapper.ApplyFinalFilters();
        }
    }
}
