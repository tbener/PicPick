using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models.Interfaces;
using PicPick.Models.IsDirtySupport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using TalUtils;

namespace PicPick.Models
{
    public partial class PicPickProjectActivityDestination : IPath, ICloneable
    {

        public PicPickProjectActivityDestination(IActivity activity) : this()
        {
            Activity = activity;
        }

        public static PicPickProjectActivityDestination CreateNew(IActivity activity)
        {
            PicPickProjectActivityDestination destination = new PicPickProjectActivityDestination(activity)
            {
                Path = "",
                Template = "yyy-MM"
            };

            return destination;
        }

        [XmlIgnore]
        public IActivity Activity { get; set; }

        [XmlIgnore]
        public bool HasTemplate { get => !string.IsNullOrEmpty(Template); }

        public string GetTemplatePath(DateTime dt)
        {
            try
            {
                if (HasTemplate)
                    return dt.ToString(Template);
            }
            catch
            {
            }
            return string.Empty;
        }

        public string GetFullPath(DateTime joinDateTime)
        {
            return PathHelper.GetFullPath(GetFullPath(), GetTemplatePath(joinDateTime));
        }

        public string GetFullPath()
        {
            return System.IO.Path.GetFullPath(PathHelper.GetFullPath(Activity.Source.Path, Path));
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


}
