using PicPick.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Models
{
    public class ActivityFileMapping : Dictionary<string, Dictionary<string, DestinationMapping>>
    {
    }

    public class DestinationMapping
    {
        public string FullPath { get; set; }
        public bool IsNew { get; set; }
        public bool Created { get; set; }
        public List<DestinationFiles> Files { get; set; }
    }

    public class DestinationFiles
    {
        public string OriginalName { get; set; }
        public bool Exists { get; set; }
        public string NewName { get; set; }
        public FILE_STATUS Status { get; set; }
        public Exception Exception { get; set; }
    }
}
