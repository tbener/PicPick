using PicPick.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PicPick.ViewModel.UserControls
{
    public class ImageInfoViewModel : IDisposable
    {
        public ImageInfoViewModel(string imagePath)
        {
            ImageFileInfo imageInfo = new ImageFileInfo(true);

            if (ImageFileInfo.IsImage(imagePath))
                Source = imageInfo.BitmapImage(imagePath);
            else
                Source = imageInfo.AssociatedImage(imagePath);

            try
            {
                ImagePath = imagePath;
                imageInfo.SetFileStream(imagePath);
                ImageSize = imageInfo.FileSize(imagePath);

                DateTime dateTaken;
                if (imageInfo.GetFileDate(imagePath, out dateTaken))
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

        public ImageSource Source { get; set; }

        public string ImageDate { get; set; }
        public string ImageSize { get; set; }
        public string ImagePath { get; set; }

        public void Dispose()
        {
            Source = null;
        }

        #endregion
    }
}
