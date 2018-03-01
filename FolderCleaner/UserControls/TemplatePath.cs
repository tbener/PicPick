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
using System.IO;
using FolderCleaner.Configuration;

namespace FolderCleaner.UserControls
{
    public partial class TemplatePath : UserControl
    {
        public event EventHandler Changed;

        private DateTime? _previewDate;
        private FolderCleanerConfigTaskDestination _destination;

        public TemplatePath(FolderCleanerConfigTaskDestination destination)
        {
            InitializeComponent();

            _destination = destination;
            pathControl.Text = _destination.Path;
            txtTemplate.Text = _destination.Template;

            pathControl.ComboBox.TextChanged += Control_TextChanged;
            txtTemplate.TextChanged += Control_TextChanged;

            Refresh();
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            _destination.Path = pathControl.Text;
            _destination.Template = txtTemplate.Text;
            Refresh();

            Changed?.Invoke(this, new EventArgs());
        }

        public override void Refresh()
        {
            base.Refresh();

            try
            {
                lblPreview.Text = _destination.GetFullPath(PreviewDate.Value);
            }
            catch (Exception ex)
            {
                lblPreview.Text = $"({ex.Message})";
            }
        }


        public DateTime? PreviewDate
        {
            get => _previewDate.GetValueOrDefault(DateTime.Now);
            set
            {
                _previewDate = value;
                Refresh();
            }
        }
        
    }
}
