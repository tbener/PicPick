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

        private string _text = null;
        private string _header;
        private int _value;
        private int _maximum;
        private CancellationTokenSource cts;

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
            if (UserCancelled)
            {
                Text = "Cancelled";
            }
            else if (Exception != null)
            {
                Text = "Error occured";
            }
            else
                Text = "Done";

            if (cts != null)
                cts.Dispose();

            Report();
        }

        public void Reset()
        {
            Value = 0;
            Maximum = 0;
            Header = "";
            Text = "";
            Exception = null;
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
            RaisePropertyChanged(nameof(Text));
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

        public string ProgressPercentsText => Maximum > 0 & Value > 0 ? $"{(100*Value)/Maximum}%" : "";

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
