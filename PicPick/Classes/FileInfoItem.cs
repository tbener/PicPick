using PicPick.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Classes
{

    

    internal class FileInfoItem
    {
        public FileInfoItem()
        {
            Status = FILE_STATUS.NONE;
        }

        public FileInfoItem(DateTime dateTime)
        {
            this.DateTime = dateTime;
        }
        public DateTime DateTime { get; set; }
        public FILE_STATUS Status { get; set; }
    }
    
}
