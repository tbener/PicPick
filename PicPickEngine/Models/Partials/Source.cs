using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PicPick.Models
{
    public partial class PicPickProjectActivitySource : IPath, ICloneable
    {

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        
    }


}
