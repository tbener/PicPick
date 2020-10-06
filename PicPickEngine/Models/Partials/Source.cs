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

        public static PicPickProjectActivitySource CreateNew()
        {
            try
            {
                PicPickProjectActivitySource source = new PicPickProjectActivitySource();
                source.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                source.FromDate = new DateComplex();
                source.FromDate.Date = DateTime.Today.AddDays(-30);
                source.ToDate = new DateComplex();
                source.ToDate.Date = DateTime.Today;

                return source;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        
    }


}
