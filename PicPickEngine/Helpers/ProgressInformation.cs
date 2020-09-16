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
    public class ProgressInformation : IProgressInformation
    {

        #region Private members

        private string _text = null;
        private string _header;
        private int _value;
        private int _maximum;
        private CancellationTokenSource cts;
        private bool _finished;

        #endregion

        #region CTOR

        public ProgressInformation()
        {
            Progress = new Progress<ProgressInformation>();
        }


        #endregion

        #region Public Methods

        public void Finished()
        {
            if (OperationCancelled)
            {
                Text = "Cancelled";
            }
            else if (Exception != null)
            {
                Text = "Error occured";
            }
            else
                Text = "";

            if (cts != null)
                cts.Dispose();

            _finished = true;
            Report();
            RaisePropertyChanged(nameof(IsWorking));
        }

        public void Reset()
        {
            Value = 0;
            Maximum = 100;
            Header = "";
            Text = "";
            Exception = null;
            OperationCancelled = false;
            _finished = false;

            Report();

            RenewToken();
            RaisePropertyChanged(nameof(IsWorking));
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                RaisePropertyChanged("Header");
            }
        }

        public int Maximum
        {
            get => _maximum;
            set
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

        public void AdvanceWithCancellationToken(int step = 1)
        {
            Advance(step);
            CancellationToken.ThrowIfCancellationRequested();
        }

        public void Report()
        {
            Progress.Report(this);
            RaisePropertyChanged(nameof(ProgressPercentsText));
            RaisePropertyChanged(nameof(Text));
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
            get => _value;
            set
            {
                int prev = _value;
                _value = value;
                RaisePropertyChanged("Value");
                if (prev == 0)
                    RaisePropertyChanged(nameof(IsWorking));
            }
        }

        public bool IsWorking
        {
            get
            {
                return !_finished && Value > 0;
            }
        }

        public IProgress<ProgressInformation> Progress { get; set; }

        public string ProgressPercentsText => !_finished & Maximum > 0 & Value > 0 ? $"{100 * Value / Maximum}%" : "";

        public int CurrentOperationTotal { get; internal set; }
        public FileExistsResponseEnum FileExistsResponse { get; set; }
        public Dictionary<FILE_STATUS, List<string>> Summary { get; set; }
        public bool OperationCancelled { get; set; }

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
