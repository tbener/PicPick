using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicPick.Core;
using PicPick.Models.Interfaces;

namespace PicPick.Models
{
    public partial class PicPickProject_options : IOptions
    {
        public FileExistsResponseEnum FileExistsResponse
        {
            get
            {
                FileExistsResponseEnum fileExistsResponse = FileExistsResponseEnum.ASK;
                Enum.TryParse<FileExistsResponseEnum>(FileExistsResponseString, out fileExistsResponse);
                return fileExistsResponse;
            }
            set
            {
                FileExistsResponseString = value.ToString();
            }
        }
    }
}
