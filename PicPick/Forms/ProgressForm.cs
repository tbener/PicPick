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
        bool _canClose = false;

        public ProgressForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            lblMain.Text = "Initializing...";
            lblStatus.Text = "";
            progressBar.Value = 0;
            _canClose = false;
        }


        public void ShowResults(string msg)
        {
            lblStatus.Text = msg;
            progressBar.Value = 0;
            btnCancel.Text = "Close";
            btnCancel.Enabled = true;
            _canClose = true;
        }

        public void Refresh(ProgressInformation info)
        {
            progressBar.Value = info.CountDone;
            Application.DoEvents();

            if (info.Done)
            {
                btnCancel.Text = "Close";
                btnCancel.Enabled = true;
                _canClose = true;
                if (info.Exception != null)
                {
                    lblMain.Text = "Finished with errors";
                    lblStatus.Text = info.Exception.Message;
                }
                else
                {
                    lblMain.Text = "Done";
                    lblStatus.Text = "";
                }
            }
            else if (info.CountDone == 0)
            {
                lblMain.Text = "initializing...";
                lblStatus.Text = "";
            }
            else
            {
                lblMain.Text = info.MainOperation;
                lblStatus.Text = $"{(progressBar.Value * 100) / info.Total}%";
            }

            base.Refresh();
        }

        private void Progress_ProgressChanged(object sender, ProgressInformation info)
        {
            Refresh(info);
        }

        public int Max { get => progressBar.Maximum; set => progressBar.Maximum = value; }

        public Progress<ProgressInformation> Progress
        {
            get => _progress;
            set
            {
                if (_progress != null)
                    _progress.ProgressChanged -= Progress_ProgressChanged;
                _progress = value;
                _progress.ProgressChanged += Progress_ProgressChanged;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_canClose)
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
