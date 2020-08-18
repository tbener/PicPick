using PicPick.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    /// <summary>
    /// This class holds the main dictionary (Files) that 
    /// - its Key is the Source File path
    /// - its Value is a SourceFile class
    /// Every SourceFile class splits into its destinations, and then to the destination files.
    /// 
    /// These classes are updated during the mapping process and then during the running porcess itself.
    /// </summary>
    public class FilesGraph
    {
        // MAIN PROPERTY
        public Dictionary<string, SourceFile> Files { get; set; }

        //Helper properties
        public Dictionary<string, DestinationFolder> DestinationFolders { get; set; } = new Dictionary<string, DestinationFolder>();
        public HashSet<string> RawFileList { get; set; }

        public void Clear()
        {
            Files = null;
            DestinationFolders.Clear();
            RawFileList = null;
        }
    }
}
