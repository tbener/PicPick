using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Classes
{
    public class ProgressInformation
    {
        public string FileCopied { get; set; }
        public string DestinationFolder { get; set; }

        public static int Total { get; set; }
        public int CountDone { get; set; }
        public int Percents()
        {
            return (CountDone * 100) / Total;
        }

        public void Advance()
        {
            CountDone += 1;
        }
    }
}
