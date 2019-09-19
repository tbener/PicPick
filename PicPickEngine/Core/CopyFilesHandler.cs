using PicPick.Helpers;
using PicPick.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Core
{
    public delegate void FileProcessEventHandler(object sender, string file, string msg, bool success = true);
    public delegate void FileStatusChangedEventHandler(object sender, string fileFullName, FILE_STATUS status);
    public delegate FileExistsResponseEnum FileExistsEventHandler(object sender, FileExistsAskEventArgs eventArgs);

    
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
        const FileExistsResponseEnum DEFAULT_FILE_EXISTS_RESPONSE = FileExistsResponseEnum.SKIP;

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

        #region CTOR

        public CopyFilesHandler(string dest, List<string> fileList)
        {
            DestinationFolder = dest;
            FileList = fileList;
            Status = COPY_STATUS.NOT_STARTED;
        }
        public CopyFilesHandler(string dest) : this(dest, new List<string>())
        { }

        #endregion

        public void AddFile(string file)
        {
            if (!FileList.Contains(file))
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

        #region Copying

        // this is static because an activity (task) can use a few instances of this class
        public static FileExistsResponseEnum FileExistsResponse { get; set; }

        internal async Task DoCopyAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            //FileExistsResponseEnum currentAction = FileExistsResponse;
            FileExistsResponseEnum nextConflictResponse = FileExistsResponse;
            FILE_STATUS fileStatus;
            string fileName = "";
            string fullFileName = "";
            progressInfo.DestinationFolder = DestinationFolder;

            try
            {
                FileExistsAskEventArgs fileExistsAskEventArgs = new FileExistsAskEventArgs();
                FileExistsResponseEnum currentConflictResponse = nextConflictResponse;

                // iterate FileList and copy to dest
                foreach (string file in FileList)
                {
                    fileName = Path.GetFileName(file);
                    fullFileName = file;
                    fileStatus = FILE_STATUS.NONE;
                    string dest = Path.Combine(DestinationFolder, fileName);

                    progressInfo.CurrentOperation = $"Copying {fileName}";
                    progressInfo.Report();

                    // if the file exists in destination
                    if (File.Exists(dest))
                    {
                        currentConflictResponse = nextConflictResponse;

                        if (currentConflictResponse == FileExistsResponseEnum.ASK)
                        {
                            fileExistsAskEventArgs.SourceFile = fullFileName;
                            fileExistsAskEventArgs.DestinationFolder = DestinationFolder;
                            // Publish the event
                            currentConflictResponse = EventAggregatorHelper.PublishFileExists(fileExistsAskEventArgs);

                            if (fileExistsAskEventArgs.Cancel)
                                return;

                            if (fileExistsAskEventArgs.DontAskAgain)
                                nextConflictResponse = currentConflictResponse;
                        }
                        
                        if (currentConflictResponse == FileExistsResponseEnum.COMPARE)
                        {
                            if (AreSameFiles(file, dest))
                                currentConflictResponse = FileExistsResponseEnum.SKIP;  
                            else
                                currentConflictResponse = FileExistsResponseEnum.RENAME;
                        }

                        switch (currentConflictResponse)
                        {
                            case FileExistsResponseEnum.OVERWRITE:
                                await Task.Run(() => File.Copy(file, dest, true));
                                fileStatus = FILE_STATUS.COPIED;
                                break;
                            case FileExistsResponseEnum.SKIP:
                                fileStatus = FILE_STATUS.SKIPPED;
                                break;
                            case FileExistsResponseEnum.RENAME:

                                dest = GetNewFileName(dest);

                                await Task.Run(() => File.Copy(file, dest, true));
                                fileStatus = FILE_STATUS.COPIED;
                                break;
                            default:
                                break;
                        }

                        ReportFileProcess(fileName, currentConflictResponse, dest);
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
            catch (OperationCanceledException)
            {
                throw;
            }
            //catch (Exception ex)
            //{
            //    ReportFileProcess(fileName, $"ERROR: {ex.Message}", log4net.Core.Level.Error);
            //    OnFileStatusChanged?.Invoke(this, fullFileName, FILE_STATUS.ERROR);
            //    if (!ErrorHandler.Handle(ex, "An error occurred. Do you want to continue to the next files?"))
            //        return;
            //}
        }

 

        private bool AreSameFiles(string file1, string file2)
        {
            try
            {
                int file1byte;
                int file2byte;
                FileStream fs1;
                FileStream fs2;

                // Open the two files.
                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                // Check the file sizes. If they are not the same, the files 
                // are not the same.
                if (fs1.Length != fs2.Length)
                {
                    // Close the file
                    fs1.Close();
                    fs2.Close();

                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                int i = 0;
                do
                {
                    i++;
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1) && (i<5000000));

                // Close the files.
                fs1.Close();
                fs2.Close();

                // Return the success of the comparison. "file1byte" is 
                // equal to "file2byte" at this point only if the files are 
                // the same.
                return ((file1byte - file2byte) == 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while comparing files", ex);
            }
        }

        private string GetNewFileName(string fullPath)
        {
            int count = 2;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0} ({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;

        }

        private void ReportFileProcess(string file, string msg, log4net.Core.Level level)
        {
            try
            {
                OnFileProcess?.Invoke(this, file, msg, level == log4net.Core.Level.Info);

                LogHandler.Log(file, msg, level);
            }
            catch
            {   // todo
                throw;
            }
        }

        private void ReportFileProcess(string file, FileExistsResponseEnum action, string dest)
        {
            string msg = "";
            log4net.Core.Level level = log4net.Core.Level.Info;
            switch (action)
            {
                case FileExistsResponseEnum.OVERWRITE:
                    msg = $"Copied as {dest} (OVERWRITE)";
                    break;
                case FileExistsResponseEnum.SKIP:
                    // todo: add to log!
                    msg = $"Skipped ({dest} exists)";
                    level = log4net.Core.Level.Warn;
                    break;
                case FileExistsResponseEnum.RENAME:
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
        public string DestinationFolder { get; set; }
        public List<string> FileList { get; set; }
        public Exception Exception { get; set; }

        public string GetStatusString()
        {
            return _statusStrings[Status];
        }


    }

}
