using PicPick.Core;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel.UserControls.Mapping
{
    public class MappingStatusViewModel
    {
        string displayTemplate = "[status_name] ([files_count])";

        public MappingStatusViewModel(FILE_STATUS status, List<DestinationFile> files)
        {
            string statusDisplayTmp = displayTemplate.Replace("[status_name]", GetStatusName(status));
            StatusDisplay = statusDisplayTmp.Replace("[files_count]", files.Count.ToString());

            FileList = files.Select(f => new MappingFileViewModel(f)).ToList();
        }

        private string GetStatusName(FILE_STATUS status)
        {
            if (status == FILE_STATUS.NONE)
                return "Not processed";
            return status.ToString();
        }

        public string StatusDisplay { get; set; }
        public List<MappingFileViewModel> FileList { get; set; }
    }
}
