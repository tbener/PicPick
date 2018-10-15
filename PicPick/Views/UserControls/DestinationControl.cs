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
        private BindingSource _bindingSource;

        public DestinationControl(PicPickConfigTaskDestination destination)
        {
            InitializeComponent();

            Destination = destination;
            Destination.PropertyChanged += (s, e) => Refresh();

            pathControl.DataBindings.Add("Text", Destination, "Path", false, DataSourceUpdateMode.OnPropertyChanged);
            txtTemplate.DataBindings.Add("Text", Destination, "Template", false, DataSourceUpdateMode.OnPropertyChanged);
            chkActive.DataBindings.Add("Checked", Destination, "Active");

            Refresh();
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
            chkActiveOld.ImageIndex = chkActiveOld.ImageIndex == 0 ? 1 : 0;
            Destination.Active = (chkActiveOld.ImageIndex == 1);
            Changed?.Invoke(this, new EventArgs());
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveButtonClicked?.Invoke(this, e);
        }
    }
}
