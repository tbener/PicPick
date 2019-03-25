using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class FileExistsEventArgs : EventArgs
    {
        public FileExistsEventArgs(FileExistsResponseEnum defaultResponse)
        {
            CurrentResponse = defaultResponse;
            NextResponse = defaultResponse;
        }

        public string SourceFile { get; set; }
        public string DestinationFolder { get; set; }
        public FileExistsResponseEnum CurrentResponse { get; set; }
        public FileExistsResponseEnum NextResponse { get; set; }
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
}
