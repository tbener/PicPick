using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Classes
{
    public class ProgressInformation
    {

        public ProgressInformation()
        {
            Progress = new Progress<ProgressInformation>();
        }

        public string FileCopied { get; set; }
        public string DestinationFolder { get; set; }

        public Exception Exception { get; set; }

        public int CountDone { get; set; }

        public IProgress<ProgressInformation> Progress { get; set; }

        public void Advance()
        {
            CountDone += 1;
            Report();
        }

        public void Report()
        {
            Progress.Report(this);
        }

        public bool Done { get; set; }

        public void Start()
        {
            Done = false;
            CountDone = 0;
            Exception = null;
        }

        private string _currentOperation = null;
        public string CurrentOperation
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

        public string MainOperation { get; set; }
        public string Activity { get; set; }
        public int CurrentOperationTotal { get; internal set; }
        public int Total { get; internal set; }
    }
}
