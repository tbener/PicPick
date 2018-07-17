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

        public Exception Exception { get; set; }

        public int CountDone { get; set; }
        //public static int Total { get; set; }
        //public int Percents()
        //{
        //    return (CountDone * 100) / Total;
        //}

        public void Advance()
        {
            CountDone += 1;
        }

        public bool Done { get; set; }

        public void Start()
        {
            Done = false;
            CountDone = 0;
            Exception = null;
        }

        private string _currentOperation = null;
        public string CurrentOperationString
        {
            get {
                string s = _currentOperation == null ? $"Copying to {DestinationFolder}" : _currentOperation;
                _currentOperation = null;
                return s;
            }
            set
            {
                _currentOperation = value;
            }
        }
    }
}
