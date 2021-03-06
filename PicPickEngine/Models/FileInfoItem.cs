﻿using PicPick.Core;
using System;

namespace PicPick.Models
{
    
    public class PicPickFileInfo
    {
        public PicPickFileInfo()
        {
            Status = FILE_STATUS.NONE;
        }

        public PicPickFileInfo(DateTime dateTime) : this()
        {
            this.DateTime = dateTime;
        }

        public DateTime DateTime { get; set; }
        public FILE_STATUS Status { get; set; }
    }
    
}
