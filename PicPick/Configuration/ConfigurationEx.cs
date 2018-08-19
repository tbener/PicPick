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
using PicPick.Classes;

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

        public void SetDirty()
        {
            Initialized = false;
        }

        Dictionary<string, FileInfoItem> _dicFiles = new Dictionary<string, FileInfoItem>();
        List<string> _errorFiles = new List<string>();

        public async Task<int> GetFileCount(CancellationToken cancellationToken)
        {
            HashSet<string> fileList = new HashSet<string>();

            List<string> lst = new List<string>();
            string[] filters = Source.Filter.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                string[] fileEntries = new string[0];
                for (int i = 0; i < 100; i++)
                {
                    fileEntries = await Task.Run(() => Directory.GetFiles(Source.Path, filter));
                }
                cancellationToken.ThrowIfCancellationRequested();
                lst.AddRange(fileEntries);
            }

            fileList = new HashSet<string>(lst);
            return fileList.Count();
        }

        /*
         * Lists the files and their dates
         */
        public void ReadFiles()
        {
            DateTime dateTime = DateTime.MinValue;

            _dicFiles.Clear();
            _errorFiles.Clear();

            ImageFileInfo fileDateInfo = new ImageFileInfo();
            string[] filters = Source.Filter.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                Debug.Print($"-------------\r\nFilter: {filter}\r\n---------------");
                string[] fileEntries = Directory.GetFiles(Source.Path, filter);
                foreach (string file in fileEntries)
                {
                    if (!_dicFiles.ContainsKey(file) && !_errorFiles.Contains(file))
                        if (fileDateInfo.GetFileDate(file, out dateTime))
                            _dicFiles.Add(file, new FileInfoItem(dateTime));
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

        /*
         * Lists the files and their dates
         */
        private async Task ReadFilesAsync(CancellationToken cancellationToken)
        {

            DateTime dateTime = DateTime.MinValue;

            _dicFiles.Clear();
            _errorFiles.Clear();

            ImageFileInfo fileDateInfo = new ImageFileInfo();
            string[] filters = Source.Filter.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                Debug.Print($"-------------\r\nFilter: {filter}\r\n---------------");
                string[] fileEntries = await Task.Run(() => Directory.GetFiles(Source.Path, filter));
                cancellationToken.ThrowIfCancellationRequested();
                foreach (string file in fileEntries)
                {
                    if (!_dicFiles.ContainsKey(file) && !_errorFiles.Contains(file))
                        if (fileDateInfo.GetFileDate(file, out dateTime))
                            _dicFiles.Add(file, new FileInfoItem(dateTime));
                        else
                            _errorFiles.Add(file);
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

        Dictionary<string, CopyFilesHandler> _mapping = new Dictionary<string, CopyFilesHandler>();

        /// <summary>
        /// Create the Mapping structure.
        /// the Mapping is a list of CopyFilesHandler objects.
        /// every CopyFilesHandler object holds a list of files that should be copied to a single folder.
        /// This destination folder also used as the Mapping key.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> MapFilesAsync(CancellationToken cancellationToken)
        {
            _mapping.Clear();
            await ReadFilesAsync(cancellationToken);

            foreach (PicPickConfigTaskDestination destination in Destination)
            {
                if (!destination.Active)
                    continue;

                string pathAbsolute = PathHelper.GetFullPath(Source.Path, destination.Path);
                if (destination.HasTemplate)
                    foreach (var kv in _dicFiles)
                    {
                        // create the string from template
                        string relPath = destination.GetTemplatePath(kv.Value.DateTime);
                        string fullPath = PathHelper.GetFullPath(pathAbsolute, relPath);
                        // add to mapping dictionary
                        if (!_mapping.ContainsKey(fullPath))
                        {
                            // new destination path
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
                cancellationToken.ThrowIfCancellationRequested();
            }

            Destination.Last().Move = true;

            CountTotal = 0;
            foreach (var map in _mapping.Values)
            {
                CountTotal += map.FileList.Count();
            }

            Initialized = true;

            return true;
        }


        /// <summary>
        /// Executes a whole task. Copying the files to ALL destinations
        /// </summary>
        /// <param name="progressInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            // Initialize. Fills the Mapping dictionary
            //if (!Initialized)
            //{
            //////////////////////
            // if you don't recall MapFiles every time make sure to reset FileInfoItem.Status values
            //////////////////////
            await MapFilesAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            //}

            progressInfo.Activity = Name;
            progressInfo.Total = CountTotal;
            try
            {
                // loop on destinations
                foreach (var kv in _mapping)
                {
                    CopyEventArgs e = new CopyEventArgs(kv.Value);

                    CopyFilesHandler copyFilesHandler = kv.Value;
                    copyFilesHandler.OnFileProcess += CopyFilesHandler_OnFileProcess;
                    copyFilesHandler.OnFileStatusChanged += CopyFilesHandler_OnFileStatusChanged;
                    string fullPath = PathHelper.GetFullPath(kv.Key, true);
                    copyFilesHandler.SetStart();
                    OnCopyStatusChanged?.Invoke(this, e);

                    progressInfo.MainOperation = $"Copying to {fullPath}";
                    progressInfo.CurrentOperationTotal = copyFilesHandler.FileList.Count();
                    progressInfo.Report();

                    Debug.Print("Copying {0} files to {1}", copyFilesHandler.FileList.Count(), fullPath);
                    await copyFilesHandler.DoCopyAsync(progressInfo, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (DeleteSourceFiles)
                {
                    progressInfo.MainOperation = "Cleaning up...";
                    var copiedFileList = _dicFiles.Where(f => f.Value.Status == FILE_STATUS.COPIED).Select(f => f.Key).ToList();
                    Debug.Print($"Moving {copiedFileList.Count()} files to backup ({PathHelper.AppPath("backup")})");
                    string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
                    ShellFileOperation.DeleteCompletelySilent(backupPath);
                    ShellFileOperation.MoveItems(copiedFileList, PathHelper.GetFullPath(backupPath, true));
                }

            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                progressInfo.Exception = ex;
                
                ErrorHandler.Handle(ex, $"Error in operation: {progressInfo.MainOperation}.");
            }
            finally
            {
                progressInfo.Done = true;
                await Task.Run(() => progressInfo.Report());
            }
        }

        private void CopyFilesHandler_OnFileStatusChanged(object sender, string fileFullName, FILE_STATUS status)
        {
            // if status is COPIED we set it only if it didn't fail before
            if (status == FILE_STATUS.COPIED)
                // so if it was set we don't touch it
                if (_dicFiles[fileFullName].Status != FILE_STATUS.NONE)
                    return;
            _dicFiles[fileFullName].Status = status;
        }

        private void CopyFilesHandler_OnFileProcess(object sender, string file, string msg, bool success = true)
        {
            // if success we set it only if it didn't fail before
            //if (success)
            //    // so if it exists we don't touch it
            //    if (_dicFilesResult.ContainsKey(file))
            //        return;
            //_dicFilesResult[file] = success;
        }

        [XmlIgnore]
        public int CountTotal { get; private set; }

        [XmlIgnore]
        public Dictionary<string, CopyFilesHandler> Mapping { get => _mapping; }

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
