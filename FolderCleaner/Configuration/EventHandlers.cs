using FolderCleaner.Helpers;
using System;

namespace FolderCleaner.Configuration
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