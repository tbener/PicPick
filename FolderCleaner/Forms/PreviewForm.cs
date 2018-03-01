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

                foreach (var dst in _task.Destination)
                {
                    dst.OnCopyStatusChanged += Dst_OnCopyStatusChanged;
                    foreach (var map in dst.Mapping.Values)
                    {
                        _mapRows[map] = dgInfo.Rows.Add(dst.GetPath(map.Folder), map.FileList.Count());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error initalizing task");
            }
        }

        private void Dst_OnCopyStatusChanged(object sender, CopyEventArgs e)
        {
            dgInfo.Rows[_mapRows[e.Info]].Cells["Status"].Value = e.Info.GetStatus();
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
            if (_task != null)
            {
                if (!_task.Initialized) Preview(this);

                _task.Execute();
            }
        }
    }
}
