using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Models.IsDirtySupport
{
    public interface IIsDirtySupport
    {
        IsDirtySupport<IIsDirtySupport> GetIsDirtyInstance();
    }
}
