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
using PicPick.Configuration;

namespace PicPick.UserControls
{
    public partial class DestinationControl : UserControl
    {
        public event EventHandler Changed;
        public event EventHandler RemoveButtonClicked;

        private DateTime? _previewDate;
        private PicPickConfigTaskDestination _destination;

        public DestinationControl(PicPickConfigTaskDestination destination)
        {
            InitializeComponent();

            Destination = destination;
            pathControl.Text = Destination.Path;
            txtTemplate.Text = Destination.Template;

            pathControl.ComboBox.TextChanged += Control_TextChanged;
            txtTemplate.TextChanged += Control_TextChanged;

            Refresh();
        }

        private void Control_TextChanged(object sender, EventArgs e)
        {
            Destination.Path = pathControl.Text;
            Destination.Template = txtTemplate.Text;
            Refresh();

            Changed?.Invoke(this, new EventArgs());
        }

        public override void Refresh()
        {
            base.Refresh();

            try
            {
                lblPreview.Text = Destination.GetFullPath(PreviewDate.Value);
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

        public PicPickConfigTaskDestination Destination { get => _destination; set => _destination = value; }

        private void button2_Click(object sender, EventArgs e)
        {
            chkActive.ImageIndex = chkActive.ImageIndex == 0 ? 1 : 0;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveButtonClicked?.Invoke(this, e);
        }
    }
}
