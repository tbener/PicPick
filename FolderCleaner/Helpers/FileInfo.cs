using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FolderCleaner.Helpers
{
    internal class FileDateInfo
    {
        BitmapImageCheck _bitmapImageCheck;

        public FileDateInfo()
        {
            _bitmapImageCheck = new BitmapImageCheck();
        }

        internal bool GetFileDate(string file, out DateTime dateTime)
        {
            try
            {
                string ext = Path.GetExtension(file);
                if (_bitmapImageCheck.IsExtensionSupported(ext))
                    if (TryGetDateTaken(file, out dateTime))
                        return true;
                dateTime = File.GetLastWriteTime(file);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print($"{Path.GetFileName(file)} - {ex.Message}");
                dateTime = DateTime.MinValue;
                return false;
            }
        }

        private bool TryGetDateTaken(string file, out DateTime dateTime)
        {
            FileStream picStream = null;
            try
            {
                picStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                BitmapSource bitSource = BitmapFrame.Create(picStream);
                BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
                return DateTime.TryParse(metaData.DateTaken, out dateTime);

                //JpegBitmapDecoder decoder = new JpegBitmapDecoder(picStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                //BitmapMetadata metaData = new BitmapMetadata("jpg");
                //BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0]);

            }
            catch (Exception ex)
            {
                Debug.Print($"{Path.GetFileName(file)} - {ex.Message}");
                dateTime = DateTime.MinValue;
            }
            finally
            {
                picStream?.Close();
            }
            return false;
        }
    }
}
