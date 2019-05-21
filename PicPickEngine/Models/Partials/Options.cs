using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicPick.Core;

namespace PicPick.Models
{
    public partial class PicPickProjectOptions : IOptions
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
