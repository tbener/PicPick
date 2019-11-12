using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TalUtils;

namespace PicPick.Helpers
{
    public class ImageFileInfo
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        BitmapImageCheck _bitmapImageCheck;
        FileStream _fileStream = null;
        string _fileName = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keepFileOpen">Will keep the FileStream open until explicitly closed or other one is opened.</param>
        public ImageFileInfo(bool keepFileOpen)
        {
            _bitmapImageCheck = new BitmapImageCheck();
            KeepFileOpen = keepFileOpen;
        }

        public ImageFileInfo() : this(false)
        { }

        public BitmapImage BitmapImage(string file)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(file);
                image.EndInit();
                return image;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, false, $"Error getting image to display for the file: {file}");
                return null;
            }
        }

        public ImageSource AssociatedImage(string file)
        {
            ImageSource icon;

            try
            {
                using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(file))
                {
                    icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                              sysicon.Handle,
                              System.Windows.Int32Rect.Empty,
                              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
                return icon;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, false, $"Error getting image to display for the file: {file}");
                return null;
            }
        }

        public bool IsImage(string file)
        {
            string ext = Path.GetExtension(file);
            return _bitmapImageCheck.IsExtensionSupported(ext);
        }

        public bool GetFileDate(string file, out DateTime dateTime)
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
                _errorHandler.Handle(ex, false, "Error extracting date from file (setting minimum sate)");
                dateTime = DateTime.MinValue;
                return false;
            }
        }

        public bool TryGetDateTaken(string fileName, out DateTime dateTime)
        {
            BitmapSource bitSource;
            BitmapMetadata metaData;
            try
            {
                SetFileStream(fileName);
                bitSource = BitmapFrame.Create(_fileStream);
                metaData = (BitmapMetadata)bitSource.Metadata;
                return DateTime.TryParse(metaData.DateTaken, out dateTime);

                //JpegBitmapDecoder decoder = new JpegBitmapDecoder(picStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                //BitmapMetadata metaData = new BitmapMetadata("jpg");
                //BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0]);

            }
            catch (Exception ex)
            {
                Debug.Print($"{Path.GetFileName(fileName)} - {ex.Message}");
                dateTime = DateTime.MinValue;
            }
            finally
            {
                bitSource = null;
                metaData = null;
                if (!KeepFileOpen)
                    CloseFileStream();
            }
            return false;
        }

        public string FileSize(string fileName)
        {
            double len = GetFileLength(fileName);
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            string fmt = order < 2 ? "{0}" : "{0:0.##}";

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format(fmt + " {1}", len, sizes[order]);
        }


        /// <summary>
        /// To avoid opening the file multiple times, use this method, that in combination with GetFileStream,
        /// keeps the stream alive
        /// </summary>
        /// <param name="fileName"></param>
        public void SetFileStream(string fileName)
        {
            CloseFileStream();
            _fileName = fileName;
            _fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public FileStream GetFileStream(string fileName)
        {
            if (!_fileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                SetFileStream(fileName);

            return _fileStream;
        }

        public void CloseFileStream()
        {
            _fileName = "";
            _fileStream?.Dispose();
        }

        private long GetFileLength(string fileName)
        {
            // if there is a FileStream open, we use it
            if (_fileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                return _fileStream.Length;

            return new FileInfo(fileName).Length;
        }



        public bool KeepFileOpen { get; set; }
    }
}
