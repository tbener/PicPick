using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalUtils;
using PicPick.Helpers;

namespace PicPick.UserControls
{
    public partial class PathBrowser : UserControl
    {
        HistoryComboHelper _historyComboHelper = null;

        public event EventHandler Changed;

        public PathBrowser()
        {
            InitializeComponent();

            txtPath.AutoCompleteSource = AutoCompleteSource.FileSystem;

            ComboBox.TextChanged += ComboBox_TextChanged;
            btnOpenFolder.Enabled = PathHelper.Exists(txtPath.Text);

        }

        private void PathBrowser_Load(object sender, EventArgs e)
        {
            //btnOpenFolder.Left = this.Width - btnOpenFolder.Width;
            //btnOpenFolder.Top = 0;

            //btnBrowse.Width = btnOpenFolder.Width;
            //btnBrowse.Left = btnOpenFolder.Left - 5 - btnBrowse.Width;
            //btnBrowse.Top = 0;

            //txtPath.Left = 0;
            //txtPath.Width = btnBrowse.Left - 5;
            //txtPath.Top = 0;
        }

        private void ComboBox_TextChanged(object sender, EventArgs e)
        {
            btnOpenFolder.Enabled = PathHelper.Exists(txtPath.Text);
            Changed?.Invoke(this, e);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;
            if (DialogHelper.BrowseOpenFolderDialog(ref path)){
                txtPath.Text = path;
            }

        }

        public override string Text { get => txtPath.Text; set => txtPath.Text = value; }

        public ComboBox ComboBox { get => txtPath; }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text;
            if (PathHelper.Exists(txtPath.Text))
                Utils.OpenPath(path);
            else
                Msg.ShowE("Path doesn't exist.");
        }

        
        
    }
}
