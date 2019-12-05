using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PicPick.Helpers;
using PicPick.Models;
using TalUtils;
using PicPick.Models.Interfaces;
using log4net;
using System.IO;

namespace PicPick.Core
{
    public class Runner
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        public event CopyEventHandler OnCopyStatusChanged;

        private IActivity _activity;
        private IOptions _options;

        public FileExistsResponseEnum FileExistsResponse { get; set; }


        public Runner(IActivity activity, IOptions options)
        {
            _activity = activity;
            _options = options;
        }

        public async Task Run(ProgressInformation progressInfo)
        {
            
            progressInfo.Text = "Start copying...";
            _log.Info($"Starting: {_activity.Name}"); 
            
            FileExistsAskEventArgs fileExistsAskEventArgs = new FileExistsAskEventArgs();
            FileExistsResponseEnum currentConflictResponse;
            FileExistsResponse = _options.FileExistsResponse;

            var map = _activity.FileMapping;

            try
            {
                progressInfo.Maximum = map.SourceFiles.Count * map.Destinations.Count;
                progressInfo.Value = 0;

                foreach (DestinationFolder destinationFolder in map.DestinationFolders.Values)
                {
                    _log.Info($"Destination: {destinationFolder.FullPath}");
                    if (!Directory.Exists(destinationFolder.FullPath))
                        Directory.CreateDirectory(destinationFolder.FullPath);

                    foreach (DestinationFile destinationFile in destinationFolder.Files)
                    {
                        try
                        {
                            SourceFile sourceFile = destinationFile.SourceFile;
                            _log.Info($"-- Source file: {sourceFile.FileName}");

                            progressInfo.Text = sourceFile.FileName;
                            progressInfo.Report();

                            if (destinationFile.Exists)
                            {
                                currentConflictResponse = FileExistsResponse;
                                _log.Info($"-- File exists in destination. Response = {currentConflictResponse}");

                                if (currentConflictResponse == FileExistsResponseEnum.ASK)
                                {
                                    fileExistsAskEventArgs.SourceFile = sourceFile.FullPath;
                                    fileExistsAskEventArgs.DestinationFolder = destinationFolder.FullPath;
                                    // Publish the event
                                    currentConflictResponse = EventAggregatorHelper.PublishFileExists(fileExistsAskEventArgs);

                                    _log.Info($"-- User selected: {currentConflictResponse}");

                                    if (currentConflictResponse == FileExistsResponseEnum.ASK)
                                        // This will happen if the event wasn't handled, or wasn't handled correctly
                                        currentConflictResponse = FileExistsResponseEnum.SKIP;

                                    if (fileExistsAskEventArgs.Cancel)
                                        throw new OperationCanceledException(progressInfo.CancellationToken);

                                    if (fileExistsAskEventArgs.DontAskAgain)
                                    {
                                        _log.Info($"   + Dont Ask Again!");
                                        FileExistsResponse = currentConflictResponse;
                                    }
                                }

                                if (currentConflictResponse == FileExistsResponseEnum.COMPARE)
                                {
                                    if (FileSystemHelper.AreSameFiles(sourceFile.FullPath, destinationFile.GetFullName()))
                                    {
                                        _log.Info($"-- Comparison result: Same.");
                                        currentConflictResponse = FileExistsResponseEnum.SKIP;
                                    }
                                    else
                                    {
                                        _log.Info($"-- Comparison result: Not same.");
                                        currentConflictResponse = FileExistsResponseEnum.RENAME;
                                    }
                                }

                                _log.Info($"-- Action: {currentConflictResponse}");

                                switch (currentConflictResponse)
                                {
                                    case FileExistsResponseEnum.OVERWRITE:
                                        await DoCopy(sourceFile, destinationFile);
                                        break;
                                    case FileExistsResponseEnum.SKIP:
                                        destinationFile.SetStatus(FILE_STATUS.SKIPPED);
                                        break;
                                    case FileExistsResponseEnum.RENAME:
                                        destinationFile.NewName = FileSystemHelper.GetNewFileName(destinationFolder.FullPath, sourceFile.FileName);
                                        await DoCopy(sourceFile, destinationFile);
                                        break;
                                    default:
                                        break;
                                }

                            }
                            else
                            {
                                await DoCopy(sourceFile, destinationFile, false);
                            }

                            // report progress
                            progressInfo.Advance();
                            progressInfo.CancellationToken.ThrowIfCancellationRequested();
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            destinationFile.SetStatus(FILE_STATUS.ERROR);
                            destinationFile.Exception = ex;

                            _errorHandler.Handle(ex, false, $"An error occurred while processing the file: {destinationFile.SourceFile.FileName}");

                            // Publish the error
                            // The wrapper can take this event and make a decision whether or not to continue to the next file.
                            FileErrorEventArgs args = new FileErrorEventArgs(destinationFile);
                            bool cancel = EventAggregatorHelper.PublishFileError(args);
                            if (cancel)
                                throw ex;
                        }
                    }
                }

                if (_activity.DeleteSourceFiles)
                {
                    progressInfo.Text = "Cleaning up...";
                    var copiedFileList = _activity.FileMapping.SourceFiles.Values.Where(f => f.Status == FILE_STATUS.COPIED).Select(f => f.FullPath).ToList();
                    string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
                    _log.Info($"Moving {copiedFileList.Count()} files to backup folder ({backupPath})");
                    if (Directory.Exists(backupPath))
                        ShellFileOperation.DeleteCompletelySilent(backupPath);
                    Directory.CreateDirectory(backupPath);
                    ShellFileOperation.MoveItems(copiedFileList, backupPath);
                }

            }
            finally
            {
                
            }
        }

        private async Task DoCopy(SourceFile sourceFile, DestinationFile destinationFile, bool overwrite = true)
        {
            await Task.Run(() => File.Copy(sourceFile.FullPath, destinationFile.GetFullName(), overwrite));
            destinationFile.SetStatus(FILE_STATUS.COPIED);
            _log.Info($"---- Copied to: {destinationFile.GetFullName()}");
        }

        //public async Task Run2(ProgressInformation progressInfo, CancellationToken cancellationToken)
        //{
        //    _log.Info($"Starting: {_activity.Name}");
        //    if (_activity.IsRunning) throw new Exception("Activity is already running.");
        //    EventAggregatorHelper.PublishActivityStarted();

        //    Analyzer analyzer = new Analyzer(_activity);

        //    try
        //    {
        //        _activity.IsRunning = true;

        //        // Initialize.Fills the Mapping dictionary
        //        if (!_activity.Initialized)
        //        {
        //            // When using UI, the Analyze is usually called beforehand, as it gives the initial progress and status information.
        //            await analyzer.CreateMapping(progressInfo, cancellationToken);
        //            cancellationToken.ThrowIfCancellationRequested();
        //        }

        //        // init progress
        //        int countTotal = 0;
        //        foreach (var map in _activity.Mapping.Values)
        //        {
        //            countTotal += map.FileList.Count();
        //        }
        //        progressInfo.Maximum = countTotal;
        //        progressInfo.Value = 0;

        //        CopyFilesHandler.FileExistsResponse = _options.FileExistsResponse;
        //        progressInfo.FileExistsResponse = _options.FileExistsResponse;

        //        // loop on destinations
        //        foreach (var kv in _activity.Mapping)
        //        {
        //            CopyEventArgs e = new CopyEventArgs(kv.Value);

        //            CopyFilesHandler copyFilesHandler = kv.Value;
        //            //copyFilesHandler.OnFileProcess += CopyFilesHandler_OnFileProcess;
        //            copyFilesHandler.OnFileStatusChanged += CopyFilesHandler_OnFileStatusChanged;
        //            string fullPath = PathHelper.GetFullPath(kv.Key, true);
        //            copyFilesHandler.SetStart();
        //            OnCopyStatusChanged?.Invoke(this, e);

        //            progressInfo.MainOperation = $"Copying to {fullPath}";
        //            progressInfo.CurrentOperationTotal = copyFilesHandler.FileList.Count();
        //            progressInfo.Report();

        //            _log.InfoFormat("Copying {0} files to {1}", copyFilesHandler.FileList.Count(), fullPath);

        //            // # DO THE ACTUAL COPY
        //            await copyFilesHandler.DoCopyAsync(progressInfo, cancellationToken);

        //            cancellationToken.ThrowIfCancellationRequested();
        //        }

        //        if (_activity.DeleteSourceFiles)
        //        {
        //            progressInfo.MainOperation = "Cleaning up...";
        //            var copiedFileList = _activity.FilesInfo.Where(f => f.Value.Status == FILE_STATUS.COPIED).Select(f => f.Key).ToList();
        //            Debug.Print($"Moving {copiedFileList.Count()} files to backup ({PathHelper.AppPath("backup")})");
        //            string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
        //            ShellFileOperation.DeleteCompletelySilent(backupPath);
        //            ShellFileOperation.MoveItems(copiedFileList, PathHelper.GetFullPath(backupPath, true));
        //        }

        //    }
        //    catch (OperationCanceledException)
        //    {
        //        _log.Info("The user cancelled the operation");
        //        progressInfo.UserCancelled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        progressInfo.Exception = ex;

        //        _errorHandler.Handle(ex, false, $"Error in operation: {progressInfo.MainOperation}.");
        //    }
        //    finally
        //    {
        //        _log.Info($"Finished: {_activity.Name}");
        //        progressInfo.Finished();
        //        await Task.Run(() => progressInfo.Report());
        //        _activity.IsRunning = false;
        //        _activity.Initialized = false;
        //        EventAggregatorHelper.PublishActivityEnded();
        //    }

        //}

        //private void CopyFilesHandler_OnFileStatusChanged(object sender, string fileFullName, FILE_STATUS status)
        //{
        //    // if status is COPIED we set it only if it didn't fail before
        //    if (status == FILE_STATUS.COPIED)
        //        // so if it was set we don't touch it
        //        if (_activity.FilesInfo[fileFullName].Status != FILE_STATUS.NONE)
        //            return;
        //    _activity.FilesInfo[fileFullName].Status = status;
        //}
    }
}
