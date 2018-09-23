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
        private bool _showExplorerButton;

        public event EventHandler Changed;

        public PathBrowser()
        {
            InitializeComponent();

            cboPath.AutoCompleteSource = AutoCompleteSource.FileSystem;

            ComboBox.TextChanged += ComboBox_TextChanged;
            btnOpenFolder.Enabled = PathHelper.Exists(cboPath.Text);

            ShowExplorerButton = true;

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
            btnOpenFolder.Enabled = PathHelper.Exists(cboPath.Text);
            Changed?.Invoke(this, e);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ShowOpenFolderDialog();
        }

        public void ShowOpenFolderDialog()
        {
            string path = cboPath.Text;
            if (DialogHelper.BrowseOpenFolderDialog(ref path, DialogHeader))
            {
                cboPath.Text = path;
            }
        }

        public override string Text { get => cboPath.Text; set => cboPath.Text = value; }

        
        public ComboBox ComboBox { get => cboPath; }

        public new ControlBindingsCollection DataBindings { get => cboPath.DataBindings; }

        public string DialogHeader { get; set; }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string path = cboPath.Text;
            if (PathHelper.Exists(cboPath.Text))
                Utils.OpenPath(path);
            else
                Msg.ShowE("Path doesn't exist.");
        }

        public bool ShowExplorerButton { get => tableLayoutPanel.ColumnStyles[2].Width > 0; set => tableLayoutPanel.ColumnStyles[2].Width = (value ? tableLayoutPanel.ColumnStyles[1].Width : 0); }



    }
}
