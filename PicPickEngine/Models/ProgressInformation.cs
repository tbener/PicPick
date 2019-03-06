using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Models
{
    public class ProgressInformation : INotifyPropertyChanged
    {

        #region Private members

        private string _currentOperation = null;
        private string _mainOperation;
        private int _countDone;
        private int _total;

        #endregion

        #region CTOR

        public ProgressInformation()
        {
            Progress = new Progress<ProgressInformation>();
        }


        #endregion

        #region Public properties

        public string FileCopied { get; set; }
        public string DestinationFolder { get; set; }

        public Exception Exception { get; set; }

        public int CountDone
        {
            get => _countDone; set
            {
                _countDone = value;
                RaisePropertyChanged("CountDone");
            }
        }

        public IProgress<ProgressInformation> Progress { get; set; }

        public bool Done { get; set; }

        public string CurrentOperation
        {
            get
            {
                string s = _currentOperation == null ? $"Copying to {DestinationFolder}" : _currentOperation;
                _currentOperation = null;
                return s;
            }
            set
            {
                _currentOperation = value;
                RaisePropertyChanged("CurrentOperation");
            }
        }

        public string MainOperation
        {
            get => _mainOperation;
            set
            {
                _mainOperation = value;
                RaisePropertyChanged("MainOperation");
            }
        }
        public string Activity { get; set; }
        public int CurrentOperationTotal { get; internal set; }
        public int Total
        {
            get => _total;
            internal set
            {
                _total = value;
                RaisePropertyChanged("Total");
            }
        }

        #endregion

        #region Methods

        public void Advance()
        {
            CountDone += 1;
            Report();
        }

        public void Report()
        {
            Progress.Report(this);
        }

        public void Start()
        {
            Done = false;
            CountDone = 0;
            Exception = null;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
