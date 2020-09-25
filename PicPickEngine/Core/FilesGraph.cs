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
        private Mapper _mapper;

        // MAIN PROPERTY
        public List<SourceFile> Files { get; set; }

        //Helper properties
        public List<DestinationFolder> DestinationFolders
        {
            get
            {
                return _mapper.GetDestinationFolders();
            }
        }

        internal HashSet<string> RawFileList { get; set; }

        public void Clear()
        {
            Files = null;
            RawFileList = null;
        }

        internal void SetMapper(Mapper mapper)
        {
            _mapper = mapper;
        }
    }
}
