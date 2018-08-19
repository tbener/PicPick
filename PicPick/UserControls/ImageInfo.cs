using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PicPick.Helpers;

namespace PicPick.UserControls
{
    public partial class ImageInfo : UserControl
    {
        ImageFileInfo _imageInfo;

        public ImageInfo()
        {
            InitializeComponent();
            _imageInfo = new ImageFileInfo(true);
        }

        private void ImageInfo_Load(object sender, EventArgs e)
        {

        }

        public override void Refresh()
        {
            base.Refresh();

            lblPath.Text = ImagePath;
            lblSize.Text = "Size: ";
            lblDate.Text = "Date taken: ";

            if (ImagePath == string.Empty)
                return;

            try
            {
                pictureBox.Image = Image.FromFile(ImagePath);
                _imageInfo.SetFileStream(ImagePath);
                lblSize.Text += _imageInfo.FileSize(ImagePath);

                DateTime dateTaken;
                if (_imageInfo.TryGetDateTaken(ImagePath, out dateTaken))
                    lblDate.Text += dateTaken.ToShortDateString();
            }
            catch 
            {
                // do nothing
            }
            finally
            {
                _imageInfo.CloseFileStream();
            }
        }

        private void ImageInfo_Resize(object sender, EventArgs e)
        {
            pictureBox.Width = this.Height;
        }

        public string ImagePath
        {
            get; set;
        }
    }
}
