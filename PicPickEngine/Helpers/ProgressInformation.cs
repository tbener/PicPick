using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    public class ProgressInformation : INotifyPropertyChanged
    {

        #region Private members

        private string _currentOperation = null;
        private string _mainOperation;
        private int _value;
        private int _maximum;
        private string _activity;

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

        public int Value
        {
            get => _value; set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

        public IProgress<ProgressInformation> Progress { get; set; }

        public void ResetValues()
        {
            Value = 0;
            MainOperation = "";
            CurrentOperation = "";
            FileCopied = "";
            DestinationFolder = "";
        }

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
        public string Activity
        {
            get => _activity; set
            {
                _activity = value;
                RaisePropertyChanged(nameof(Activity));
            }
        }
        public int CurrentOperationTotal { get; internal set; }
        public int Maximum
        {
            get => _maximum;
            internal set
            {
                _maximum = value;
                RaisePropertyChanged("Maximum");
            }
        }

        #endregion

        #region Methods

        public void Advance(int step = 1)
        {
            Value += step;
            Report();
        }

        public void Report()
        {
            Progress.Report(this);
        }

        public void Start()
        {
            Value = 0;
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
