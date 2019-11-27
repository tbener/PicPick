using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using PicPick.Core;
using PicPick.Helpers;

namespace PicPick.Models.Interfaces
{
    public interface IActivity
    {
        bool DeleteSourceFiles { get; set; }
        bool Initialized { get; set; }
        bool IsRunning { get; set; }
        string Name { get; set; }

        PicPickProjectActivitySource Source { get; set; }
        ObservableCollection<PicPickProjectActivityDestination> DestinationList { get; }

        ActivityFileMapping FileMapping { get; set; }
        Runner Runner { get; set; }
    }
}