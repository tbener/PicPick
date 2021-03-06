﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class FileExistsAskEvent : PubSubEvent<FileExistsAskEventArgs> { }
    
    public class ActivityStartedEvent : PubSubEvent { }

    public class ActivityEndedEvent : PubSubEvent { }

    public class GotDirtyEvent : PubSubEvent { }

    public class FileErrorEvent : PubSubEvent<FileErrorEventArgs> { }

    public class GeneralErrorEvent : PubSubEvent<ExceptionEventArgs> { }
}
