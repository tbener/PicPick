using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.StateMachine
{
    public interface IStateHandler
    {
        bool IsRunning { get; }
        Task ExecuteAsync();
    }
}
