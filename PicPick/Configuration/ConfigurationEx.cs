using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace PicPick.Configuration
{


    public partial class PicPickConfigProjects
    {

        public PicPickConfigProjectsProject ProjectByName(string name)
        {
            PicPickConfigProjectsProject proj = this.Project.FirstOrDefault(p => p.Name == name);
            if (proj != null)
            {
                if (proj.Tasks == null)
                {
                    proj.Tasks = new List<PicPickConfigTask>();
                    foreach (var taskRef in proj.TaskRef)
                    {
                        PicPickConfigTask task = ConfigurationHelper.Default.Tasks.FirstOrDefault(t => t.Name == taskRef.Name);
                        if (task != null)
                            proj.Tasks.Add(task);
                    }
                }
            }

            return proj;
        }
    }

    public partial class PicPickConfigProjectsProject
    {
        [XmlIgnore]
        public List<PicPickConfigTask> Tasks { get; set; }
    }

    public partial class PicPickConfigTask
    {
        public event CopyEventHandler OnCopyStatusChanged;

        private List<PicPickConfigTaskDestination> _destList = null;

        [XmlIgnore]
        // Use this list rather than the Destination Array for easyer manipulations and editing.
        // This will be converted back to the Destination Array in ConfigurationHelper.Save()
        public List<PicPickConfigTaskDestination> DestinationList
        {
            get
            {
                if (_destList == null)
                    _destList = new List<PicPickConfigTaskDestination>(this.Destination);
                return _destList;
            }
        }

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
        Dictionary<string, bool> _dicFilesResult = new Dictionary<string, bool>();

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

        public async Task ReadFilesAsync()
        {
            //Task
            //await ReadFiles();
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {

            }
            catch (OperationCanceledException ex)
            {

                throw;
            }
            
        }

        Dictionary<string, CopyFilesHandler> _mapping = new Dictionary<string, CopyFilesHandler>();

        public bool Init(bool readFiles = false)
        {
            _mapping.Clear();
            if (readFiles)
                ReadFiles();

            foreach (PicPickConfigTaskDestination destination in Destination)
            {
                string pathAbsolute = PathHelper.GetFullPath(Source.Path, destination.Path);
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
                            _mapping.Add(fullPath, new CopyFilesHandler(fullPath));
                        }
                        _mapping[fullPath].AddFile(kv.Key);
                    }
                else
                {
                    if (!_mapping.ContainsKey(pathAbsolute))
                    {
                        _mapping.Add(pathAbsolute, new CopyFilesHandler(pathAbsolute));
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
            Init(true);
            _dicFilesResult.Clear();

            foreach (var kv in _mapping)
            {
                CopyEventArgs e = new CopyEventArgs(kv.Value);

                CopyFilesHandler copyFilesHandler = kv.Value;
                copyFilesHandler.OnFileProcess += CopyFilesHandler_OnFileProcess;
                string fullPath = PathHelper.GetFullPath(kv.Key, true);
                copyFilesHandler.SetStart();
                OnCopyStatusChanged?.Invoke(this, e);

                Debug.Print("Copying {0} files to {1}", copyFilesHandler.FileList.Count(), fullPath);
                copyFilesHandler.DoCopy();

                //try
                //{
                    
                //    if (ShellFileOperation.CopyItems(copyFilesInfo.FileList, fullPath))
                //        copyFilesInfo.SetFinished();
                //    else 
                //        copyFilesInfo.SetCancelled();
                //}
                //catch (Exception ex)
                //{
                //    copyFilesInfo.SetError(ex);
                //    throw;
                //}

                //OnCopyStatusChanged?.Invoke(this, e);

                //if (copyFilesInfo.Status == COPY_STATUS.CANCELLED)
                //    break;
            }

            try
            {
                Debug.Print($"Moving all files to backup ({PathHelper.AppPath("backup")})");
                string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
                ShellFileOperation.DeleteCompletelySilent(backupPath);
                ShellFileOperation.MoveItems(_dicFilesResult.Where(f => f.Value).Select(f => f.Key).ToList(), PathHelper.GetFullPath(backupPath, true));

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while backing up the files after the main operation has finished.");
            }
        }

        private void CopyFilesHandler_OnFileProcess(object sender, string file, string msg, bool success = true)
        {
            // if success we set it only if it didn't fail before
            if (success)
                // so if it exists we don't touch it
                if (_dicFilesResult.ContainsKey(file))
                    return;
            _dicFilesResult[file] = success;
        }

        [XmlIgnore]
        public int FileCount { get => _dicFiles.Count(); }

        [XmlIgnore]
        public Dictionary<string, CopyFilesHandler> Mapping { get => _mapping;  }

        public override string ToString() { return Name; }
    }


    


    public partial class PicPickConfigTaskDestination
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
