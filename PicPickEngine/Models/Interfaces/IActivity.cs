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
        bool Active { get; set; }
        bool DeleteSourceFiles { get; set; }
        ObservableCollection<PicPickProjectActivityDestination> DestinationList { get; }
        bool Initialized { get; set; }
        bool IsRunning { get; set; }
        string Name { get; set; }
        PicPickProjectActivitySource Source { get; set; }

        event CopyEventHandler OnCopyStatusChanged;

        Dictionary<string, CopyFilesHandler> Mapping { get; set; }

        Dictionary<string, PicPickFileInfo> FilesInfo { get; set; }

    }
}