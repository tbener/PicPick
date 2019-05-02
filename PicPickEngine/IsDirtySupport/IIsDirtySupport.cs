using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Project.IsDirtySupport
{
    public interface IIsDirtySupport
    {
        IsDirtySupport<IIsDirtySupport> GetIsDirtyInstance();
    }
}
