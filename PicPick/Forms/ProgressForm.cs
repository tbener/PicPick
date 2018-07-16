using PicPick.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicPick.Forms
{
    public partial class ProgressForm : Form
    {
        Progress<ProgressInformation> _progress = null;
        CancellationTokenSource _cts = null;
        bool _allowCloseForm = false;

        public ProgressForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            lblStatus.Text = "Initializing...";
            progressBar.Value = 0;
            _allowCloseForm = false;
        }


        public void ShowResults(string msg)
        {
            lblStatus.Text = msg;
            progressBar.Value = 0;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            _allowCloseForm = true;
        }

       
        private void Progress_ProgressChanged(object sender, ProgressInformation info)
        {
            lblStatus.Text = info.CurrentOperationString;
            progressBar.Value = info.CountDone;
        }

        public override string Text { get => lblStatus.Text; set => lblStatus.Text = value; }
        public int Max { get => progressBar.Maximum; set => progressBar.Maximum = value; }

        public Progress<ProgressInformation> Progress { get => _progress;
            set {
                if (_progress != null)
                    _progress.ProgressChanged -= Progress_ProgressChanged;
                _progress = value;
                _progress.ProgressChanged += Progress_ProgressChanged;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_allowCloseForm)
            {
                Close();
            }
            else
            {
                btnCancel.Enabled = false;
                btnCancel.Text = "Cancelling";

                _cts.Cancel();
            }
        }
    }
}
