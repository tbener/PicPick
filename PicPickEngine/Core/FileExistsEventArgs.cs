using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    public class FileExistsEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string Destination { get; set; }
    }
}
