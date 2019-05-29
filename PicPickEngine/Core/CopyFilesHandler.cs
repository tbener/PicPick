﻿using PicPick.Helpers;
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
    public delegate FileExistsResponseEnum FileExistsEventHandler(object sender, FileExistsEventArgs eventArgs);

    
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
            FileExistsResponseEnum action = FileExistsResponse;
            FILE_STATUS fileStatus;
            string fileName = "";
            string fullFileName = "";
            progressInfo.DestinationFolder = DestinationFolder;

            try
            {
                FileExistsEventArgs fileExistsEventArgs = new FileExistsEventArgs(FileExistsResponse);

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
                        // If the file exists:
                        // 1. Canceled: if not DontAskAgain returned from previous loop
                        // 2. set eventArgs
                        // 3. publish event
                        // 4. if CurrentResponse==ASK:
                        // 4.1. the subscriber should implement
                        // 4.2. return the response in CurrentResponse
                        // 4.3. the NextResponse value will appear in the CurrentResponse in the next time
                        // 5. if CurrentResponse!=ASK:
                        // 5.1. the subscriber can, but don't have to implement
                        // 6. the subscriber just needs to make sure not to return ASK as CurrentResponse
                        // 7. Canceled: if the subscriber sets DontAskAgain=true, the event will not fire again in this operation.
                        // 8. if the subscriber sets Cancel=true, the operation will stop.

                        // ###### PUBLISH FILE EXISTS EVENT
                        fileExistsEventArgs.SourceFile = fullFileName;
                        fileExistsEventArgs.DestinationFolder = DestinationFolder;
                        // Publish the event
                        action = EventAggregatorHelper.PublishFileExists(fileExistsEventArgs);

                        if (fileExistsEventArgs.Cancel)
                            return;

                        fileExistsEventArgs.CurrentResponse = fileExistsEventArgs.NextResponse;
                        // ###### END PUBLISH FILE EXISTS EVENT



                        #region old ASK

                        // If the file exists:
                        // 1. action is initialized as FileExistsResponse
                        // 2. publish FileExistsEvent in case the host wants to change the action.
                        // 3. it the action is ASK - publish the AskEvent for the host to implement it and return the chosen action.
                        // 4. if the ASK is implemented:
                        // 4.1. the action is set to chosen response.
                        // 4.2. if "Dont Ask Again" is true, then FileExistsResponse will be set to be the chosen action, and make it the active action on the next iterations.
                        // 5. if the ASK is not implemented:
                        // 5.1. the returned action would be ASK, again, and it will be set to the default as a fallback.

                        /*
                        action = EventAggregatorHelper.Publish(new FileExistsEventArgs()
                        {
                            SourceFile = fullFileName,
                            DestinationFolder = DestinationFolder,
                            CurrentResponse = FileExistsResponse,
                        });

                        if (action == FileExistsResponseEnum.ASK)
                        {
                            AskEventArgs e = new AskEventArgs()
                            {
                                SourceFile = fullFileName,
                                DestinationFolder = DestinationFolder,
                                Response = FileExistsResponse,
                                DontAskAgain = false
                            };
                            action = EventAggregatorHelper.Publish(e);
                            // as a percaution, in case the ASK was not implemented or implemented wrongly
                            if (action == FileExistsResponseEnum.ASK)
                                action = DEFAULT_FILE_EXISTS_RESPONSE;
                            if (e.DontAskAgain)
                                FileExistsResponse = action;
                        }
                        */
                        #endregion

                        if (action == FileExistsResponseEnum.COMPARE)
                        {
                            if (AreSameFiles(file, dest))
                                action = FileExistsResponseEnum.SKIP;  
                            else
                                action = FileExistsResponseEnum.RENAME;
                        }

                        switch (action)
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
