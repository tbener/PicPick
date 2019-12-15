using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingFileViewModel
    {
        public MappingFileViewModel(DestinationFile file)
        {
            FileName = file.SourceFile.FileName;
            SourceFileName = file.SourceFile.FullFileName;
        }

        public string FileName { get; set; }
        public string SourceFileName { get; set; }
    }
}
