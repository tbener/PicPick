using PicPick.Models.Mapping;
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
            DestinationFileName = string.IsNullOrEmpty(file.NewName) ? FileName : file.NewName;
            Error = file.Exception?.Message;

            DestinationFileInfo = file;
        }

        public string FileName { get; set; }
        public string SourceFileName { get; set; }
        public string DestinationFileName { get; set; }
        public string Error { get; set; }

        public DestinationFile DestinationFileInfo { get; private set; }
    }

}
