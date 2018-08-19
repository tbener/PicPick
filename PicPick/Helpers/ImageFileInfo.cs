using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PicPick.Helpers
{
    internal class ImageFileInfo
    {
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
        {}

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

        internal bool TryGetDateTaken(string fileName, out DateTime dateTime)
        {

            try
            {
                SetFileStream(fileName);
                BitmapSource bitSource = BitmapFrame.Create(_fileStream);
                BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
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
                if (!KeepFileOpen)
                    CloseFileStream();
            }
            return false;
        }

        internal string FileSize(string fileName)
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
        internal void SetFileStream(string fileName)
        {
            CloseFileStream();
            _fileName = fileName;
            _fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        internal FileStream GetFileStream(string fileName)
        {
            if (!_fileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                SetFileStream(fileName);

            return _fileStream;
        }

        internal void CloseFileStream()
        {
            _fileName = "";
            _fileStream?.Close();
        }

        internal long GetFileLength(string fileName)
        {
            // if there is a FileStream open, we use it
            if (_fileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
                return _fileStream.Length;

            return new FileInfo(fileName).Length;
        }

        

        public bool KeepFileOpen { get; set; }
    }
}
