﻿using PicPick.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class CancellableEventArgs : EventArgs
    {
        public CancellableEventArgs()
        { }

        public bool Cancel { get; set; }

    }

    public class FileExistsAskEventArgs : EventArgs
    {
        
        public string SourceFile { get; set; }
        public string DestinationFolder { get; set; }
        public FileExistsResponseEnum Response { get; set; }
        public bool Cancel { get; set; }
        public bool DontAskAgain { get; set; }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(Exception ex)
        {
            Exception = ex;
        }

        public Exception Exception { get; set; }
    }

    public class FileErrorEventArgs : EventArgs
    {
        public FileErrorEventArgs(DestinationFile destinationFile)
        {
            DestinationFile = destinationFile;
            Cancel = true;
        }

        public DestinationFile DestinationFile { get; set; }
        public bool Cancel { get; set; } 
    }

    public class CopyEventArgs : EventArgs
    {
        public CopyEventArgs()
        { }

        public CopyEventArgs(CopyFilesHandler info)
        {
            Info = info;
        }
        public CopyFilesHandler Info { get; set; }

    }

    public class ActivityStateChangedEventArgs : EventArgs
    {
        public ActivityStateChangedEventArgs()
        { }

        public bool Cancel { get; set; }

    }
}
