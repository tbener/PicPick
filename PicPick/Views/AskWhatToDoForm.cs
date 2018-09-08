using PicPick.Helpers;
using PicPick.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicPick.Views
{
    public partial class AskWhatToDoForm : Form
    {

        public AskWhatToDoForm()
        {
            InitializeComponent();

            InitActionButton(copyActionOverwrite, FILE_EXISTS_RESPONSE.OVERWRITE);
            InitActionButton(copyActionSkip, FILE_EXISTS_RESPONSE.SKIP);
            InitActionButton(copyActionKeepBoth, FILE_EXISTS_RESPONSE.RENAME);
        }

        private void InitActionButton(CopyActionDisplay actionControl, FILE_EXISTS_RESPONSE action)
        {
            actionControl.Tag = action;
            actionControl.Click += ActionControl_Click;
        }

        private void ActionControl_Click(object sender, EventArgs e)
        {
            DontAskAgain = chkDontAskAgain.Checked;
            SelectedAction = (FILE_EXISTS_RESPONSE)((CopyActionDisplay)sender).Tag;

            copyActionOverwrite.ImageInfo.ReleaseImage();
            copyActionSkip.ImageInfo.ReleaseImage();

            Close();
        }

        private void AskWhatToDoForm_Load(object sender, EventArgs e)
        {

        }

        public void ShowDialog(string fileName, string imageSource, string imageDest)
        {
            lblHeader.Text = string.Format(Properties.Resources.DLG_FILE_EXISTS_TITLE, fileName);
            lblDestPath.Text = imageDest;

            copyActionOverwrite.ImageInfo.ImagePath = Path.Combine(imageSource, fileName);
            copyActionOverwrite.ImageInfo.Refresh();
            copyActionSkip.ImageInfo.ImagePath = Path.Combine(imageDest, fileName);
            copyActionSkip.ImageInfo.Refresh();

            ShowDialog();

            
        }

        public bool DontAskAgain { get; set; }
        public FILE_EXISTS_RESPONSE SelectedAction { get; set; }
        public static CancellationTokenSource CancellationTokenSource { get; internal set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel(true);
        }
    }
}
