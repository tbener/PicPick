﻿using PicPick.Interfaces;
using PicPick.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Helpers
{
    public delegate void FileProcessEventHandler(object sender, string file, string msg, bool success = true);
    public delegate void FileStatusChangedEventHandler(object sender, string fileFullName, FILE_STATUS status);
    public delegate FILE_EXISTS_RESPONSE FileExistsEventHandler(object sender, FileExistsEventArgs eventArgs);

    public enum FILE_EXISTS_RESPONSE
    {
        [Description("Ask me")]
        ASK,        // Needs to be implemented by the host application
        [Description("Overwrite")]
        OVERWRITE,
        [Description("Skip")]
        SKIP,
        [Description("Keep both")]
        RENAME,     // Save both
        [Description("Auto decide")]
        COMPARE     // Check if same files or just same names. then, if same, skip, if not, keep them both (rename)
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
        const FILE_EXISTS_RESPONSE DEFAULT_FILE_EXISTS_RESPONSE = FILE_EXISTS_RESPONSE.SKIP;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string pathAbsolute;

        public event FileProcessEventHandler OnFileProcess;
        public event FileStatusChangedEventHandler OnFileStatusChanged;
        public event FileExistsEventHandler OnFileExists;

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
            Destination = dest;
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

        // this is static because an activity (task) can use a few instances of this class
        public static FILE_EXISTS_RESPONSE FileExistsResponse { get; set; }

        internal async Task DoCopyAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            FILE_EXISTS_RESPONSE action = FileExistsResponse;
            FILE_STATUS fileStatus;
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
                        // If the file exists:
                        // 1. action is initialized as FileExistsResponse
                        // 2. publish FileExistsEvent in case the host wants to change the action.
                        // 3. it the action is ASK - publish the AskEvent for the host to implement it and return the chosen action.
                        // 4. if the ASK is implemented:
                        // 4.1. the action is set to chosen response.
                        // 4.2. if "Dont Ask Again" is true, then FileExistsResponse will be set to be the chosen action, and make it the active action on the next iterations.
                        // 5. if the ASK is not implemented:
                        // 5.1. the returned action would be ASK, again, and it will be set to the default as a fallback.
                        action = EventServices.Publish(new FileExistsEventArgs()
                        {
                            FileName = fileName,
                            Destination = Destination,
                            Response = FileExistsResponse,
                        });

                        if (action == FILE_EXISTS_RESPONSE.ASK)
                        {
                            AskEventArgs e = new AskEventArgs()
                            {
                                FileName = fileName,
                                Destination = Destination,
                                Response = FileExistsResponse,
                                DontAskAgain = false
                            };
                            action = EventServices.Publish(e);
                            // as a percaution, in case the ASK was not implemented or implemented wrongly
                            if (action == FILE_EXISTS_RESPONSE.ASK)
                                action = DEFAULT_FILE_EXISTS_RESPONSE;
                            if (e.DontAskAgain)
                                FileExistsResponse = action;
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

        //AskWhatToDoForm askWhatToDoForm;
        public static IFileExistsDialog FileExistsDialog { get; set; }

        private FILE_EXISTS_RESPONSE AskWhatToDo(string fileName, string sourcePath, string destPath, out bool dontAskAgain)
        {
            //if (askWhatToDoForm == null)
            //    askWhatToDoForm = new AskWhatToDoForm();

            FileExistsDialog.ShowDialog(fileName, sourcePath, destPath);

            dontAskAgain = FileExistsDialog.DontAskAgain;
            return FileExistsDialog.SelectedAction;
        }

        private bool AreSameFiles(string f1, string f2)
        {
            return false;
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
