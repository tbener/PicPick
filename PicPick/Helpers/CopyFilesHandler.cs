using PicPick.Classes;
using PicPick.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Helpers
{
    public delegate void FileProcessEventHandler(object sender, string file, string msg, bool success=true);
    public delegate void FileStatusChangedEventHandler(object sender, string fileFullName, FILE_STATUS status);

    public enum FILE_EXISTS_RESPONSE
    {
        ASK,
        OVERWRITE,
        SKIP,
        RENAME,     // save both
        COMPARE     // check if same files or just same names. then act accordingly...
    }

    public enum FILE_STATUS
    {
        NONE,
        COPIED,
        SKIPPED,
        ERROR
    }

    public enum COPY_STATUS
    {
        NOT_STARTED = 0,
        STARTED = 1,
        FINISHED = 2,
        CANCELLED = 3,
        ERROR = 9
    }

    

    public class CopyFilesHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event FileProcessEventHandler OnFileProcess;
        public event FileStatusChangedEventHandler OnFileStatusChanged;

        static readonly Dictionary<COPY_STATUS, string> _statusStrings = new Dictionary<COPY_STATUS, string>()
            {
                { COPY_STATUS.NOT_STARTED, "" },
                { COPY_STATUS.STARTED, "Started" },
                { COPY_STATUS.FINISHED, "Finished" },
                { COPY_STATUS.CANCELLED, "Cancelled" },
                { COPY_STATUS.ERROR, "Error" }
            };

        public CopyFilesHandler(string dest, List<string> fileList)
        {
            Destination = dest;
            FileList = fileList;
            Status = COPY_STATUS.NOT_STARTED;
        }
        public CopyFilesHandler(string dest) : this(dest, new List<string>())
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

        public void DoCopy()
        {
            FILE_EXISTS_RESPONSE fileExistsResponse = FILE_EXISTS_RESPONSE.ASK;
            FILE_EXISTS_RESPONSE action = fileExistsResponse;
            bool dontAsk = false;
            string fileName = "";

            try
            {
                // iterate FileList and copy to dest
                foreach (string file in FileList)
                {
                    fileName = Path.GetFileName(file);
                    string dest = Path.Combine(Destination, fileName);

                    // if the file exists in destination
                    if (File.Exists(dest))
                    {
                        action = fileExistsResponse;
                        if (fileExistsResponse == FILE_EXISTS_RESPONSE.ASK)
                        {
                            // ask the user.
                            // the choices are: Skip, Overwrite or Rename (keep both)
                            // todo: action = AskWhatToDo();

                            // temp. because the UI is not supported yet...
                            action = FILE_EXISTS_RESPONSE.SKIP;

                            // temp. because the UI is not supported yet...
                            dontAsk = true;

                            if (dontAsk)
                                // make this permanent
                                fileExistsResponse = action;
                        }

                        if (action == FILE_EXISTS_RESPONSE.COMPARE)
                        {
                            // todo:
                            // check other properties. if it seems like its the same file - overwrite
                            // if not - rename

                            if (AreSameFiles(file, dest))
                                action = FILE_EXISTS_RESPONSE.SKIP;  // we can overwrite, but for testing purposes...
                            else
                                action = FILE_EXISTS_RESPONSE.RENAME;

                            // temp. because we don't support comparing
                            action = FILE_EXISTS_RESPONSE.SKIP;
                        }

                        switch (action)
                        {
                            case FILE_EXISTS_RESPONSE.OVERWRITE:
                                File.Copy(file, dest, true);
                                break;
                            case FILE_EXISTS_RESPONSE.SKIP:
                                break;
                            case FILE_EXISTS_RESPONSE.RENAME:
                                // todo: get a new file name

                                //temp
                                dest = Path.Combine(TalUtils.PathHelper.GetFullPath(Destination, "Existing Files", true), Path.GetFileName(file));

                                File.Copy(file, dest, true);
                                break;
                            default:
                                break;
                        }
                        ReportFileProcess(fileName, action, dest);
                    }
                    else
                    {
                        File.Copy(file, dest);
                        ReportFileProcess(fileName, $"Copied to {dest}", log4net.Core.Level.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportFileProcess(fileName, $"ERROR: {ex.Message}", log4net.Core.Level.Error);
                if (!ErrorHandler.Handle(ex, "An error occurred. Do you want to continue to the next files?"))
                    return;
            }

        }

        internal async Task DoCopyAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            FILE_EXISTS_RESPONSE fileExistsResponse = FILE_EXISTS_RESPONSE.ASK;
            FILE_EXISTS_RESPONSE action = fileExistsResponse;
            FILE_STATUS fileStatus;
            bool dontAsk = false;
            string fileName = "";
            string fullFileName = "";
            progressInfo.DestinationFolder = Destination;

            try
            {
                // iterate FileList and copy to dest
                foreach (string file in FileList)
                {
                    fileName = Path.GetFileName(file);
                    fullFileName = file;
                    fileStatus = FILE_STATUS.NONE;
                    string dest = Path.Combine(Destination, fileName);

                    progressInfo.CurrentOperation = $"Copying {fileName}";
                    progressInfo.Report();

                    // if the file exists in destination
                    if (File.Exists(dest))
                    {
                        action = fileExistsResponse;
                        if (fileExistsResponse == FILE_EXISTS_RESPONSE.ASK)
                        {
                            // ask the user.
                            // the choices are: Skip, Overwrite or Rename (keep both)
                            // todo: action = AskWhatToDo();

                            // temp. because the UI is not supported yet...
                            action = FILE_EXISTS_RESPONSE.SKIP;

                            // temp. because the UI is not supported yet...
                            dontAsk = true;

                            if (dontAsk)
                                // make this permanent
                                fileExistsResponse = action;
                        }

                        if (action == FILE_EXISTS_RESPONSE.COMPARE)
                        {
                            // todo:
                            // check other properties. if it seems like its the same file - overwrite
                            // if not - rename

                            if (AreSameFiles(file, dest))
                                action = FILE_EXISTS_RESPONSE.SKIP;  // we can overwrite, but for testing purposes...
                            else
                                action = FILE_EXISTS_RESPONSE.RENAME;

                            // temp. because we don't support comparing
                            action = FILE_EXISTS_RESPONSE.SKIP;
                        }

                        switch (action)
                        {
                            case FILE_EXISTS_RESPONSE.OVERWRITE:
                                await Task.Run(() => File.Copy(file, dest, true));
                                fileStatus = FILE_STATUS.COPIED;
                                break;
                            case FILE_EXISTS_RESPONSE.SKIP:
                                fileStatus = FILE_STATUS.SKIPPED;
                                break;
                            case FILE_EXISTS_RESPONSE.RENAME:
                                // todo: get a new file name

                                //temp
                                dest = Path.Combine(TalUtils.PathHelper.GetFullPath(Destination, "Existing Files", true), Path.GetFileName(file));

                                await Task.Run(() => File.Copy(file, dest, true));
                                fileStatus = FILE_STATUS.COPIED;
                                break;
                            default:
                                break;
                        }
                        
                        ReportFileProcess(fileName, action, dest);
                    }
                    else
                    {
                        await Task.Run(() => File.Copy(file, dest));
                        fileStatus = FILE_STATUS.COPIED;
                        ReportFileProcess(fileName, $"Copied to {dest}", log4net.Core.Level.Info);
                    }

                    OnFileStatusChanged?.Invoke(this, file, fileStatus);

                    // report progress
                    progressInfo.FileCopied = fileName;
                    progressInfo.Advance();

                    // check cancellation token
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                ReportFileProcess(fileName, $"ERROR: {ex.Message}", log4net.Core.Level.Error);
                OnFileStatusChanged?.Invoke(this, fullFileName, FILE_STATUS.ERROR);
                if (!ErrorHandler.Handle(ex, "An error occurred. Do you want to continue to the next files?"))
                    return;
            }
        }

        private bool AreSameFiles(string f1, string f2)
        {
            return false;
        }

        private void ReportFileProcess(string file, string msg, log4net.Core.Level level)
        {
            try
            {
                OnFileProcess?.Invoke(this, file, msg, level == log4net.Core.Level.Info);
                
                LogHandler.Log(file, msg, level);
            }
            catch (Exception ex)
            {   // todo
                throw;
            }
        }

        private void ReportFileProcess(string file, FILE_EXISTS_RESPONSE action, string dest)
        {
            string msg = "";
            log4net.Core.Level level = log4net.Core.Level.Info;
            switch (action)
            {
                case FILE_EXISTS_RESPONSE.OVERWRITE:
                    msg = $"Copied as {dest} (OVERWRITE)";
                    break;
                case FILE_EXISTS_RESPONSE.SKIP:
                    // todo: add to log!
                    msg = $"Skipped ({dest} exists)";
                    level = log4net.Core.Level.Warn;
                    break;
                case FILE_EXISTS_RESPONSE.RENAME:
                    msg = $"Copied as {dest}";
                    break;
                default:
                    break;
            }
            ReportFileProcess(file, msg, level);
        }

        #endregion

        public COPY_STATUS Status { get; private set; }
        //public string RelativePath { get; set; }
        public string Destination { get; set; }
        public List<string> FileList { get; set; }
        public Exception Exception { get; set; }

        public string GetStatusString()
        {
            return _statusStrings[Status];
        }

        
    }
    
}
