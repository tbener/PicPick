using FolderCleaner.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace FolderCleaner.Configuration
{


    public partial class FolderCleanerConfigProjects
    {

        public FolderCleanerConfigProjectsProject ProjectByName(string name)
        {
            FolderCleanerConfigProjectsProject proj = this.Project.FirstOrDefault(p => p.Name == name);
            if (proj != null)
            {
                if (proj.Tasks == null)
                {
                    proj.Tasks = new List<FolderCleanerConfigTask>();
                    foreach (var taskRef in proj.TaskRef)
                    {
                        FolderCleanerConfigTask task = ConfigurationHelper.Default.Tasks.FirstOrDefault(t => t.Name == taskRef.Name);
                        if (task != null)
                            proj.Tasks.Add(task);
                    }
                }
            }

            return proj;
        }
    }

    public partial class FolderCleanerConfigProjectsProject
    {
        [XmlIgnore]
        public List<FolderCleanerConfigTask> Tasks { get; set; }
    }

    public partial class FolderCleanerConfigTask
    {
        public event CopyEventHandler OnCopyStatusChanged;

        [XmlIgnore]
        public TaskRunner Runner { get; set; }

        [XmlIgnore]
        public bool Initialized { get; private set; }

        private bool isDirty;

        public void SetDirty()
        {
            isDirty = true;
            Initialized = false;
        }

        Dictionary<string, DateTime> _dicFiles = new Dictionary<string, DateTime>();
        List<string> _errorFiles = new List<string>();

        /*
         * Lists the files and their dates
         */
        public void ReadFiles()
        {
            DateTime dateTime = DateTime.MinValue;

            _dicFiles.Clear();
            _errorFiles.Clear();

            FileDateInfo fileDateInfo = new FileDateInfo();
            string[] filters = Source.Filter.Split(new char[]{',', ';'},StringSplitOptions.RemoveEmptyEntries);
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                Debug.Print($"-------------\r\nFilter: {filter}\r\n---------------");
                string[] fileEntries = Directory.GetFiles(Source.Path, filter);
                foreach (string file in fileEntries)
                {
                    if (!_dicFiles.ContainsKey(file) && !_errorFiles.Contains(file))
                        if (fileDateInfo.GetFileDate(file, out dateTime))
                            _dicFiles.Add(file, dateTime);
                        else
                            _errorFiles.Add(file);
                    //Debug.Print($"{Path.GetFileName(file)} - {_dicFiles[file].ToShortDateString()}");
                }
            }
            Debug.Print($"Found {_dicFiles.Count()} files");
            if (_errorFiles.Count() > 0)
            {
                Debug.Print($"ERRORS: Couldn't get dates from {_errorFiles.Count()} files:");
                foreach (string file in _errorFiles)
                {
                    Debug.Print($"\t{Path.GetFileName(file)}");
                }
            }
        }

        Dictionary<string, CopyFilesInfo> _mapping = new Dictionary<string, CopyFilesInfo>();

        public bool Init(bool readFiles = false)
        {
            _mapping.Clear();
            if (readFiles)
                ReadFiles();

            foreach (FolderCleanerConfigTaskDestination destination in Destination)
            {
                string pathAbsolute = PathHelper.GetFullPath(Source.Path, destination.Path);
                // Create a new mapping dictionary (key: template value. value: file)
                //destination.Mapping = new Dictionary<string, CopyFilesInfo>();
                if (destination.HasTemplate)
                    foreach (var kv in _dicFiles)
                    {
                        // create the string from template
                        string relPath = destination.GetTemplatePath(kv.Value);
                        string fullPath = PathHelper.GetFullPath(pathAbsolute, relPath);
                        // add to mapping dictionary
                        if (!_mapping.ContainsKey(fullPath))
                        {
                            Debug.Print($"New template path: {relPath} (in {fullPath})");
                            _mapping.Add(fullPath, new CopyFilesInfo(relPath));
                        }
                        _mapping[fullPath].AddFile(kv.Key);
                    }
                else
                {
                    if (!_mapping.ContainsKey(pathAbsolute))
                    {
                        _mapping.Add(pathAbsolute, new CopyFilesInfo(""));
                    }
                    _mapping[pathAbsolute].AddRange(_dicFiles.Keys.ToList());
                    //_mapping.Add("", new CopyFilesInfo("", _dicFiles.Keys.ToList()));
                }
            }

            Destination.Last().Move = true;

            Initialized = true;

            return true;
        }

        public void Execute()
        {
            foreach (var kv in _mapping)
            {
                CopyEventArgs e = new CopyEventArgs(kv.Value);

                CopyFilesInfo copyFilesInfo = kv.Value;
                string fullPath = PathHelper.GetFullPath(kv.Key, true);
                copyFilesInfo.SetStart();
                OnCopyStatusChanged?.Invoke(this, e);

                try
                {
                    Debug.Print("Copying {0} files to {1}", copyFilesInfo.FileList.Count(), fullPath);
                    if (ShellFileOperation.CopyItems(copyFilesInfo.FileList, fullPath))
                        copyFilesInfo.SetFinished();
                    else 
                        copyFilesInfo.SetCancelled();
                }
                catch (Exception ex)
                {
                    copyFilesInfo.SetError(ex);
                    throw;
                }

                OnCopyStatusChanged?.Invoke(this, e);

                if (copyFilesInfo.Status == COPY_STATUS.CANCELLED)
                    break;
            }

            try
            {
                Debug.Print($"Moving all files to backup ({PathHelper.AppPath("backup")})");
                string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
                ShellFileOperation.DeleteCompletelySilent(backupPath);
                ShellFileOperation.MoveItems(_dicFiles.Keys.ToList(), PathHelper.GetFullPath(backupPath, true));

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while backing up the files after the main operation has finished.");
            }
        }

        [XmlIgnore]
        public int FileCount { get => _dicFiles.Count(); }

        [XmlIgnore]
        public Dictionary<string, CopyFilesInfo> Mapping { get => _mapping;  }

        public override string ToString() { return Name; }
    }


    public enum COPY_STATUS
    {
        NOT_STARTED = 0,
        STARTED = 1,
        FINISHED = 2,
        CANCELLED = 3,
        ERROR = 9
    }

    public class CopyFilesInfo
    {
        static readonly Dictionary<COPY_STATUS, string> _statusStrings = new Dictionary<COPY_STATUS, string>()
            {
                { COPY_STATUS.NOT_STARTED, "" },
                { COPY_STATUS.STARTED, "Started" },
                { COPY_STATUS.FINISHED, "Finished" },
                { COPY_STATUS.CANCELLED, "Cancelled" },
                { COPY_STATUS.ERROR, "Error" }
            };
        
        public CopyFilesInfo(string folder, List<string> fileList)
        {
            Folder = folder;
            FileList = fileList;
            Status = COPY_STATUS.NOT_STARTED;
        }
        public CopyFilesInfo(string folder) :  this(folder, new List<string>())
        { }

        public void AddFile(string file)
        {
            FileList.Add(file);
        }

        public void AddRange(IEnumerable<string> fileList)
        {
            FileList.AddRange(fileList);
        }

        #region Status handling

        private void SetStatus(COPY_STATUS stat, Exception ex)
        {
            Status = stat;
            Exception = ex;
        }

        public void SetStart()
        {
            SetStatus(COPY_STATUS.STARTED, null);
        }
        public void SetFinished()
        {
            SetStatus(COPY_STATUS.FINISHED, null);
        }
        public void SetCancelled()
        {
            SetStatus(COPY_STATUS.CANCELLED, null);
        }
        public void SetError(Exception ex)
        {
            SetStatus(COPY_STATUS.ERROR, ex);
        }

        #endregion

        public COPY_STATUS Status { get; private set; }
        public string Folder { get; set; }
        public List<string> FileList { get; set; }
        public Exception Exception { get; set; }

        public string GetStatusString()
        {
            return _statusStrings[Status];
        }
    }


    public partial class FolderCleanerConfigTaskDestination
    {
        public event CopyEventHandler OnCopyStatusChanged;

        [XmlIgnore]
        public Dictionary<string, CopyFilesInfo> Mapping { get; set; }

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
                CopyFilesInfo map = kv.Value;
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
