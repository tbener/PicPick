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
        bool Initialized { get; set; }
        string Name { get; set; }

        PicPickProjectActivitySource Source { get; set; }
        ObservableCollection<PicPickProjectActivityDestination> DestinationList { get; }

        //Mapper FileMapping { get; set; }
        Runner Runner { get; set; }
        FilesGraph FilesGraph { get; set; }
        StateManager StateMachine { get; }
        ActivityState State { get; set; }
        bool IsRunning { get; set; }

        event ActivityStateChangedEventHandler OnActivityStateChanged;

        PicPickProjectActivity Clone(string v);
        void ValidateFields();
    }
}