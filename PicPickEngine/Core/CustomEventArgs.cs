﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class FileExistsEventArgs : EventArgs
    {
        public string SourceFile { get; set; }
        public string DestinationFolder { get; set; }
        public FileExistsResponseEnum Response { get; set; }
    }

    public class AskEventArgs : EventArgs
    {
        public string SourceFile { get; set; }
        public string DestinationFolder { get; set; }
        public FileExistsResponseEnum Response { get; set; }
        public bool DontAskAgain { get; set; }
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
}