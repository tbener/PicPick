﻿using PicPick.Core;
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
        public static IEventAggregator EventAggregator { get; set; }

        public static FileExistsResponseEnum PublishFileExists(FileExistsAskEventArgs payload)
        {
            EventAggregator?.GetEvent<FileExistsAskEvent>().Publish(payload);
            return payload.Response;
        }

        public static void PublishActivityStarted()
        {
            EventAggregator?.GetEvent<ActivityStartedEvent>().Publish();
        }

        public static void PublishActivityEnded()
        {
            EventAggregator?.GetEvent<ActivityEndedEvent>().Publish();
        }

        public static void PublishGotDirty()
        {
            EventAggregator?.GetEvent<GotDirtyEvent>().Publish();
        }
        


        //internal static FileExistsResponseEnum Publish(AskEventArgs payload)
        //{
        //    EventAggregator?.GetEvent<AskEvent>().Publish(payload);
        //    return payload.Response;
        //}
    }
}
