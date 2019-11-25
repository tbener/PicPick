using PicPick.Core;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private IActivity _activity;
        private CancellationTokenSource cts;

        #endregion

        #region CTOR

        public ProgressInformation()
        {
            Progress = new Progress<ProgressInformation>();
        }


        #endregion

        #region Public Methods

        public void PrepareToStart(IActivity activity)
        {
            ResetValues();

            _activity = activity;
            int countTotal = 0;
            foreach (var map in activity.Mapping.Values)
            {
                countTotal += map.FileList.Count();
            }
            Maximum = countTotal;
        }

        public void Finished()
        {
            if (UserCancelled)
            {
                MainOperation = "Cancelled";
            }
            else if (Exception != null)
            {
                MainOperation = "Error occured";
            }
            else
                MainOperation = "Done";

            cts.Dispose();
            Value = 0;
        }

        public void ResetValues()
        {
            Value = 0;
            Maximum = 0;
            MainOperation = "";
            CurrentOperation = "";
            Exception = null;
        }

        public string CurrentOperation
        {
            get
            {
                return _currentOperation;
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
        
        public int Maximum
        {
            get => _maximum;
            internal set
            {
                _maximum = value;
                RaisePropertyChanged("Maximum");
            }
        }


        public void Advance(int step = 1)
        {
            Value += step;
            Report();
        }

        public void Report()
        {
            Progress.Report(this);
            RaisePropertyChanged(nameof(ProgressPercentsText));
        }

        public void Start()
        {
            Value = 0;
            Exception = null;
        }

        public void RenewToken()
        {
            cts = new CancellationTokenSource();
            CancellationToken = cts.Token;
        }

        public void Cancel()
        {
            cts.Cancel();
        }

        #endregion

        #region Public properties

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

        public string ProgressPercentsText => Maximum > 0 & Value > 0 ? $"{Value} of {Maximum}" : "";

        public int CurrentOperationTotal { get; internal set; }
        public FileExistsResponseEnum FileExistsResponse { get; set; }
        public Dictionary<FILE_STATUS, List<string>> Summary { get; set; }
        public bool UserCancelled { get; set; }

        public CancellationToken CancellationToken { get; set; }

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
