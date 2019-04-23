using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Project
{
    public interface IIsDirtySupport
    {
        //event EventHandler OnGotDirty;

        //bool IsDirty { get; set; }

        //void InvokeGotDirty();

        IsDirtySupport<IIsDirtySupport> GetIsDirtyInstance();
    }
}
