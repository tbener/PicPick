using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;

namespace PicPick.Project
{
    public partial class PicPickProject
    {
        private ObservableCollection<PicPickProjectActivity> _activityList = null;
        private string _name;
        private bool _isDirty;
        private bool _propertyChangedSupportInitlized;


        public void StartSupportFullPropertyChanged()
        {
            if (_propertyChangedSupportInitlized) return;

            // any property change (except for IsDirty...) will set IsDirty to true
            this.PropertyChanged += (s, e) =>
            {
                if (!e.PropertyName.Equals("IsDirty")) IsDirty = true;
            };

            _propertyChangedSupportInitlized = true;

        }


        [XmlIgnore]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Call StartSupportFullPropertyChanged() to support this!
        /// 
        /// Any property change (except for IsDirty...) will set IsDirty to true.
        /// The Save operation will set it to false.
        /// </summary>
        [XmlIgnore]
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                this.RaisePropertyChanged("IsDirty");
            }
        }

        // Use this list rather than the Activity Array for easyer manipulations and editing.
        // This will be converted back to the Activity Array in ConfigurationHelper.Save()
        [XmlIgnore]
        public ObservableCollection<PicPickProjectActivity> ActivityList
        {
            get
            {
                if (_activityList == null)
                {
                    _activityList = new ObservableCollection<PicPickProjectActivity>();
                    _activityList.CollectionChanged += ActivityList_CollectionChanged;
                    if (this.Activities != null)
                        foreach (PicPickProjectActivity activity in this.Activities)
                        {
                            _activityList.Add(activity);
                        }
                }
                return _activityList;
            }
        }

        private void ActivityList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PicPickProjectActivity activity in e.NewItems)
                {
                    //Added items
                    activity.StartSupportFullPropertyChanged();
                    activity.PropertyChanged += Activity_PropertyChanged;
                }
            }

            this.RaisePropertyChanged("ActivityList");
        }

        private void Activity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("Activity");
        }
    }

    public partial class PicPickProjectActivitySource
    {
        public async Task<int> GetFileCount(CancellationToken cancellationToken)
        {
            HashSet<string> fileList = new HashSet<string>();

            List<string> lst = new List<string>();
            string[] filters = this.Filter.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            // loop on filters
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                // get file list for current filter
                string[] fileEntries = await Task.Run(() => Directory.GetFiles(this.Path, filter));
                cancellationToken.ThrowIfCancellationRequested();
                // add to main file list - we're not just counting in case of duplications
                lst.AddRange(fileEntries);
            }

            // create a unique file list
            fileList = new HashSet<string>(lst);
            // return the count
            return fileList.Count();
        }
    }

    public partial class PicPickProjectActivityDestination
    {
        public event CopyEventHandler OnCopyStatusChanged;


        [XmlIgnore]
        public Dictionary<string, CopyFilesHandler> Mapping { get; set; }

        [XmlIgnore]
        public bool Move { get; set; }

        [XmlIgnore]
        public bool HasTemplate { get => !string.IsNullOrEmpty(Template); }

        [XmlIgnore]
        public string PathAbsolute { get; set; }

        public string GetTemplatePath(DateTime dt)
        {
            return HasTemplate ? dt.ToString(Template) : string.Empty;
        }

        //public string GetTemplatePath(string file)
        //{
        //    return HasTemplate ? GetTemplatePath(GetFileDate(file, true)) : string.Empty;
        //}

        //public DateTime GetFileDate(string file, bool usePicDateTaken)
        //{
        //    if (usePicDateTaken)
        //        return GetDateTaken(file);
        //    else
        //        return File.GetLastWriteTime(file);
        //}

        //public static DateTime GetDateTaken(string inFullPath)
        //{
        //    DateTime returnDateTime = DateTime.MinValue;
        //    FileStream picStream = null;
        //    try
        //    {
        //        picStream = new FileStream(inFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        BitmapSource bitSource = BitmapFrame.Create(picStream);
        //        BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
        //        returnDateTime = DateTime.Parse(metaData.DateTaken);

        //        //JpegBitmapDecoder decoder = new JpegBitmapDecoder(picStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
        //        //BitmapMetadata metaData = new BitmapMetadata("jpg");
        //        //BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0]);

        //    }
        //    catch(Exception ex)
        //    {
        //        Debug.Print($"{System.IO.Path.GetFileName(inFullPath)} - {ex.Message}");
        //        returnDateTime = File.GetLastWriteTime(inFullPath);
        //    }
        //    finally
        //    {
        //        picStream?.Close();
        //    }
        //    return returnDateTime;
        //}

        public string GetFullPath(DateTime dt)
        {
            return PathHelper.GetFullPath(Path, GetTemplatePath(dt), false);
        }

        internal string GetPath(string path, bool buildPath = false)
        {
            return PathHelper.GetFullPath(PathAbsolute, path, buildPath);
        }

        public void Execute()
        {
            foreach (var kv in Mapping)
            {
                CopyEventArgs e = new CopyEventArgs(kv.Value);

                // get the full path and CREATE it if not exists
                string fullPath = GetPath(kv.Key, true);
                CopyFilesHandler map = kv.Value;
                map.SetStart();
                OnCopyStatusChanged?.Invoke(this, e);

                try
                {

                    // The operation is done on a banch of files at once!
                    if (Move)
                    {
                        Debug.Print("Moving {0} files to {1}", map.FileList.Count(), fullPath);
                        ShellFileOperation.MoveItems(map.FileList, fullPath);
                    }
                    else
                    {
                        Debug.Print("Copying {0} files to {1}", map.FileList.Count(), fullPath);
                        ShellFileOperation.CopyItems(map.FileList, fullPath);
                    }
                    map.SetFinished();
                }
                catch (Exception ex)
                {
                    map.SetError(ex);
                    throw;
                }

                OnCopyStatusChanged?.Invoke(this, e);
            }
        }


    }


}
