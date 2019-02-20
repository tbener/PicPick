using PicPick.Helpers;
using System;

namespace PicPick.Project
{
    public delegate void CopyEventHandler(object sender, CopyEventArgs e);

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