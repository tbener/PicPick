using System;

namespace FolderCleaner.Configuration
{
    public delegate void CopyEventHandler(object sender, CopyEventArgs e);

    public class CopyEventArgs : EventArgs
    {
        public CopyEventArgs()
        { }

        public CopyEventArgs(CopyFilesInfo info)
        {
            Info = info;
        }
        public CopyFilesInfo Info { get; set; }

    }
}