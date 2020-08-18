using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    public class CoreActions
    {
        public IActivity Activity { get; private set; }

        public IAction Reader { get; set; }
        public IAction Mapper { get; set; }
        public IAction Runner { get; set; }

        public CoreActions(IActivity activity)
        {
            Activity = activity;

            Reader = new Reader(Activity);
            Mapper = new Mapper(Activity);
            Runner = new Runner(Activity);
        }
    }
}
