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
    public class StateTransition_Read : BaseStateTransition
    {
        Reader reader;

        public StateTransition_Read(StateManager manager) : base(manager)
        {
            reader = (Reader)manager.CoreActions.Reader;
        }

        protected override async Task<bool> Action()
        {
            Debug.WriteLine("Running Reading...");
            return reader.ReadFiles();
        }

    }
}
