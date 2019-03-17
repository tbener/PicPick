using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    public class EventServices
    {
        public static FILE_EXISTS_RESPONSE Publish(FileExistsEventArgs payload)
        {
            EventAggregator?.GetEvent<FileExistsEvent>().Publish(payload);
            return payload.Response;
        }

        public static IEventAggregator EventAggregator { get; set; }

        internal static FILE_EXISTS_RESPONSE Publish(AskEventArgs payload)
        {
            EventAggregator?.GetEvent<AskEvent>().Publish(payload);
            return payload.Response;
        }
    }
}
