using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PicPick.Models
{
    public partial class PicPickProjectActivitySource : IPath, ICloneable
    {
        List<string> _fileList = new List<string>();
        bool _initialized = false;
        bool _fileListUpdated = false;
        private bool _enableAutoFileListReset;

        private void DisposeFileList()
        {
            _fileList?.Clear();
            _fileListUpdated = false;
        }

        private void ReadFiles()
        {
            DisposeFileList();

            List<string> lstFiles = new List<string>();
            string[] filters = this.Filter.Replace(" ", "").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            SearchOption searchOption = IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (filters.Count() == 0 || filters.Contains("*.*"))
                filters = new []{"*.*"};

            // loop on filters
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                // get file list for current filter
                string[] fileEntries = Directory.GetFiles(this.Path, filter, searchOption);
                // add to main file list (could include duplicates)
                lstFiles.AddRange(fileEntries);
            }

            // create a unique file list
            _fileList.AddRange(new HashSet<string>(lstFiles));
            _fileListUpdated = true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [XmlIgnore]
        public List<string> FileList
        {
            get
            {
                if (!_fileListUpdated || !EnableAutoFileListReset) ReadFiles();
                return _fileList;
            }
        }

        public bool EnableAutoFileListReset
        {
            get => _enableAutoFileListReset;
            set
            {
                _enableAutoFileListReset = value;
                if (_enableAutoFileListReset && !_initialized)
                {
                    this.PropertyChanged += (s, e) =>
                    {
                        DisposeFileList();
                    };
                    _initialized = true;
                }
            }
        }
    }


}
