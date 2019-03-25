using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.ViewModel.UserControls
{
    public class ImageInfoViewModel : IDisposable
    {
        public ImageInfoViewModel(string imagePath)
        {
            ImagePath = imagePath;

            ImageFileInfo imageInfo = new ImageFileInfo(true);

            try
            {
                imageInfo.SetFileStream(ImagePath);
                ImageSize = imageInfo.FileSize(ImagePath);

                DateTime dateTaken;
                if (imageInfo.TryGetDateTaken(ImagePath, out dateTaken))
                    ImageDate = dateTaken.ToShortDateString();
            }
            catch
            {
                // do nothing
            }
            finally
            {
                imageInfo.CloseFileStream();
                imageInfo = null;
            }
        }

        #region Properties

        public string ImagePath { get; set; }
        public string ImageDate { get; set; }
        public string ImageSize { get; set; }

        public void Dispose()
        {
            //ImagePath
        }

        #endregion
    }
}
