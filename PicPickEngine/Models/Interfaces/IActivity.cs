using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models.Mapping;
using PicPick.StateMachine;

namespace PicPick.Models.Interfaces
{
    public interface IActivity
    {
        string Name { get; set; }

        PicPickProjectActivitySource Source { get; set; }
        ObservableCollection<PicPickProjectActivityDestination> DestinationList { get; }

        FilesGraph FileGraph { get; set; }
        StateManager StateMachine { get; }
        ActivityState State { get; set; }
        bool IsRunning { get; set; }

        event ActivityStateChangedEventHandler OnActivityStateChanged;

        PicPickProjectActivity Clone(string v);
        void ValidateFields();
    }
}