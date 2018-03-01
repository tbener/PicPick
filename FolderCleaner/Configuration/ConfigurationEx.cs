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

        public bool Init()
        {
            string[] fileEntries = Directory.GetFiles(Source.Path, Source.Filter);
            Debug.Print("Found {0} \"{1}\" files", fileEntries.Count(), Source.Filter);
            FileCount = fileEntries.Count();

            foreach (FolderCleanerConfigTaskDestination destination in Destination)
            {
                destination.PathAbsolute = PathHelper.GetFullPath(Source.Path, destination.Path);
                // Create a new mapping dictionary (key: template value. value: file)
                destination.Mapping = new Dictionary<string, CopyFilesInfo>();
                if (destination.HasTemplate)
                    foreach (string file in fileEntries)
                    {
                        // create the string from template
                        string relPath = destination.GetTemplatePath(file);
                        // add to mapping dictionary
                        if (!destination.Mapping.ContainsKey(relPath))
                        {
                            Debug.Print("New template path: {0}", relPath);
                            destination.Mapping.Add(relPath, new CopyFilesInfo(relPath));
                        }
                        destination.Mapping[relPath].AddFile(file);
                    }
                else
                    destination.Mapping.Add("", new CopyFilesInfo("", fileEntries.ToList()));
            }

            Destination.Last().Move = true;

            Initialized = true;

            return true;
        }

        public void Execute()
        {
            foreach (var dst in Destination)
            {
                dst.Execute();
            }
        }

        [XmlIgnore]
        public int FileCount { get; private set; }

        public override string ToString() { return Name; }
    }


    public enum COPY_STATUS
    {
        NOT_STARTED = 0,
        STARTED = 1,
        FINISHED = 2,
        ERROR = 9
    }

    public class CopyFilesInfo
    {
        static readonly Dictionary<COPY_STATUS, string> _statusStrings = new Dictionary<COPY_STATUS, string>()
            {
                { COPY_STATUS.NOT_STARTED, "" },
                { COPY_STATUS.STARTED, "Started" },
                { COPY_STATUS.FINISHED, "Finished" },
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

        #region Status handling

        public void SetStart()
        {
            Status = COPY_STATUS.STARTED;
            Exception = null;
        }
        public void SetFinished()
        {
            Status = COPY_STATUS.FINISHED;
            Exception = null;
        }
        public void SetError(Exception ex)
        {
            Status = COPY_STATUS.ERROR;
            Exception = ex;
        }

        #endregion

        private COPY_STATUS Status { get; set; }
        public string Folder { get; set; }
        public List<string> FileList { get; set; }
        public Exception Exception { get; set; }

        public string GetStatus()
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

        public string GetTemplatePath(string file)
        {
            return HasTemplate ? GetTemplatePath(GetFileDate(file, true)) : string.Empty;
        }

        public DateTime GetFileDate(string file, bool usePicDateTaken)
        {
            if (usePicDateTaken)
                return GetDateTaken(file);
            else
                return File.GetLastWriteTime(file);
        }

        public static DateTime GetDateTaken(string inFullPath)
        {
            DateTime returnDateTime = DateTime.MinValue;
            FileStream picStream = null;
            try
            {
                picStream = new FileStream(inFullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BitmapSource bitSource = BitmapFrame.Create(picStream);
                BitmapMetadata metaData = (BitmapMetadata)bitSource.Metadata;
                returnDateTime = DateTime.Parse(metaData.DateTaken);

                //JpegBitmapDecoder decoder = new JpegBitmapDecoder(picStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                //BitmapMetadata metaData = new BitmapMetadata("jpg");
                //BitmapFrame frame = BitmapFrame.Create(decoder.Frames[0]);

            }
            catch(Exception ex)
            {
                Debug.Print($"{System.IO.Path.GetFileName(inFullPath)} - {ex.Message}");
                returnDateTime = File.GetLastWriteTime(inFullPath);
            }
            finally
            {
                picStream?.Close();
            }
            return returnDateTime;
        }

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
