using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class FileExistsEvent : PubSubEvent<FileExistsEventArgs> { }
    
    public class ActivityStartedEvent : PubSubEvent { }

    public class ActivityEndedEvent : PubSubEvent { }

    public class GotDirtyEvent : PubSubEvent { }
}
