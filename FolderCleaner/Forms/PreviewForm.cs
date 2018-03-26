using FolderCleaner.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalUtils;

namespace FolderCleaner.Forms
{
    public partial class PreviewForm : Form
    {
        FolderCleanerConfigTask _task;
        Dictionary<CopyFilesInfo, int> _mapRows = new Dictionary<CopyFilesInfo, int>();
        public PreviewForm()
        {
            InitializeComponent();
        }

        public void Preview(IWin32Window owner = null)
        {
            try
            {
                dgInfo.Rows.Clear();
                _mapRows.Clear();
                this.Show(owner);

                if (_task == null) return;

                if (!_task.Initialized) _task.Init();

                _task.OnCopyStatusChanged += OnCopyStatusChanged;
                foreach (var map in _task.Mapping)
                {
                    DataGridViewRow row = dgInfo.Rows[dgInfo.Rows.Add()];
                    row.Cells["Path"].Value = map.Key;
                    row.Cells["Files"].Value = map.Value.FileList.Count();

                    _mapRows[map.Value] = row.Index;

                }
                btnRun.Enabled = true;
                
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error initalizing task");
            }
        }

        private void OnCopyStatusChanged(object sender, CopyEventArgs e)
        {
            dgInfo.Rows[_mapRows[e.Info]].Cells["Status"].Value = e.Info.GetStatusString();
        }

        public void Start(FolderCleanerConfigTask task)
        {
            _task = task;
            Preview();
        }

        public FolderCleanerConfigTask Task { get => _task; set => _task = value; }

        private void PreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            if (_task != null)
            {
                if (!_task.Initialized) Preview(this);

                _task.Execute();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
