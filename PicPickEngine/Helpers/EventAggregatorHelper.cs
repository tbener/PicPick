using PicPick.Core;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    public static class EventAggregatorHelper
    {
        public static FileExistsResponseEnum Publish(FileExistsEventArgs payload)
        {
            EventAggregator?.GetEvent<FileExistsEvent>().Publish(payload);
            return payload.Response;
        }

        public static IEventAggregator EventAggregator { get; set; }

        internal static FileExistsResponseEnum Publish(AskEventArgs payload)
        {
            EventAggregator?.GetEvent<AskEvent>().Publish(payload);
            return payload.Response;
        }
    }
}
