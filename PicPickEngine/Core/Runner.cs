using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PicPick.Helpers;
using TalUtils;
using PicPick.Models.Interfaces;
using log4net;
using System.IO;
using PicPick.Models.Mapping;
using PicPick.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PicPick.UnitTests")]
namespace PicPick.Core
{
    public class Runner : IAction
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private IOptions _options;

        public FileExistsResponseEnum FileExistsResponse { get; set; }
        public PicPickProjectActivity Activity { get; private set; }


        public Runner(IActivity activity, IOptions options)
        {
            Activity = (PicPickProjectActivity)activity;
            _options = options;
        }

        public Runner(IActivity activity) : this(activity, ProjectLoader.Project.Options)
        { }

        private FileExistsResponseEnum ResolveConflict(DestinationFile destFile, IProgressInformation progressInfo)
        {
            FileExistsResponseEnum selectedResponse = this.FileExistsResponse;

            if (this.FileExistsResponse == FileExistsResponseEnum.ASK)
            {
                FileExistsAskEventArgs fileExistsAskEventArgs = new FileExistsAskEventArgs()
                {
                    SourceFile = destFile.SourceFile.FullFileName,
                    DestinationFolder = destFile.ParentFolder.FullPath
                };

                // Publish the event
                selectedResponse = EventAggregatorHelper.PublishFileExists(fileExistsAskEventArgs);

                _log.Info($"-- User selected: {selectedResponse}");

                if (selectedResponse == FileExistsResponseEnum.ASK)
                    // This will happen if the event wasn't handled, or wasn't handled correctly
                    selectedResponse = FileExistsResponseEnum.SKIP;

                if (fileExistsAskEventArgs.Cancel)
                {
                    progressInfo.OperationCancelled = true;
                    throw new OperationCanceledException(progressInfo.CancellationToken);
                }

                if (fileExistsAskEventArgs.DontAskAgain)
                {
                    _log.Info($"   + Dont Ask Again!");
                    this.FileExistsResponse = selectedResponse;
                }
            }

            if (selectedResponse == FileExistsResponseEnum.COMPARE)
            {
                if (FileSystemHelper.AreSameFiles(destFile.SourceFile.FullFileName, destFile.GetFullName()))
                {
                    _log.Info($"-- Comparison result: Same.");
                    selectedResponse = FileExistsResponseEnum.SKIP;
                }
                else
                {
                    _log.Info($"-- Comparison result: Not same.");
                    selectedResponse = FileExistsResponseEnum.RENAME;
                }
            }

            _log.Info($"-- Final conflict action: {selectedResponse}");

            return selectedResponse;
        }

        public async Task RunAsync(IProgressInformation progressInfo)
        {

            progressInfo.Text = "Copying...";
            _log.Info($"Starting: {Activity.Name}");

            FileExistsAskEventArgs fileExistsAskEventArgs = new FileExistsAskEventArgs();
            FileExistsResponse = _options.FileExistsResponse;

            var filesGraph = Activity.FileGraph;

            try
            {
                progressInfo.Maximum = filesGraph.Files.Count * Activity.DestinationList.Where(d => d.Active).Count();
                progressInfo.Value = 0;

                HashSet<DestinationFolder> destinationFolders = new HashSet<DestinationFolder>();

                foreach (var sourceFile in filesGraph.Files)
                {
                    _log.Info($"Source file: {sourceFile.FileName}");
                    progressInfo.Text = sourceFile.FileName;

                    foreach (var destinationFolder in sourceFile.DestinationFolders)
                    {
                        _log.Info($"-- Copying to: {destinationFolder.FullPath}");
                        // for not checking the existance of a folder many times, we use the HashSet
                        if (!destinationFolders.Contains(destinationFolder))
                        {
                            destinationFolders.Add(destinationFolder);
                            if (!Directory.Exists(destinationFolder.FullPath))
                                Directory.CreateDirectory(destinationFolder.FullPath);
                        }

                        var destFile = destinationFolder.DestinationFiles[sourceFile];

                        try
                        {
                            if (destFile.Exists(true))
                            {
                                _log.Info($"-- File exists in destination. Response = {FileExistsResponse}");
                                var conflictAction = ResolveConflict(destFile, progressInfo);

                                switch (conflictAction)
                                {
                                    case FileExistsResponseEnum.OVERWRITE:
                                        await DoCopy(sourceFile, destFile);
                                        break;
                                    case FileExistsResponseEnum.SKIP:
                                        destFile.SetStatus(FILE_STATUS.SKIPPED);
                                        break;
                                    case FileExistsResponseEnum.RENAME:
                                        destFile.NewName = FileSystemHelper.GetNewFileName(destinationFolder.FullPath, sourceFile.FileName);
                                        await DoCopy(sourceFile, destFile);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                await DoCopy(sourceFile, destFile, false);
                            }

                            progressInfo.AdvanceWithCancellationToken();
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            destFile.SetStatus(FILE_STATUS.ERROR);
                            destFile.Exception = ex;

                            _errorHandler.Handle(ex, false, $"An error occurred while copying the file:\n{sourceFile.FileName}\nTo:\n{destinationFolder.FullPath}");

                            // Publish the error
                            // The wrapper can take this event and make a decision whether or not to continue to the next file.
                            FileErrorEventArgs args = new FileErrorEventArgs(destFile);
                            bool cancel = EventAggregatorHelper.PublishFileError(args);
                            if (cancel)
                                throw ex;
                        }
                    }

                    if (ShouldDeleteSourceFile(sourceFile))
                    {
                        ShellFileOperation.MoveToRecycleBin(sourceFile.FullFileName);
                    }
                }

                #region Old logic

                //foreach (DestinationFolder destinationFolder in filesGraph.DestinationFolders.Values)
                //{
                //    _log.Info($"Destination: {destinationFolder.FullPath}");
                //    if (!Directory.Exists(destinationFolder.FullPath))
                //        Directory.CreateDirectory(destinationFolder.FullPath);

                //    foreach (DestinationFile destinationFile in destinationFolder.Files)
                //    {
                //        try
                //        {
                //            SourceFile sourceFile = destinationFile.SourceFile;
                //            _log.Info($"-- Source file: {sourceFile.FileName}");

                //            progressInfo.Text = $"Copying {sourceFile.FileName}";
                //            progressInfo.Report();

                //            if (destinationFile.Exists(true))
                //            {
                //                currentConflictResponse = FileExistsResponse;
                //                _log.Info($"-- File exists in destination. Response = {currentConflictResponse}");

                //                if (currentConflictResponse == FileExistsResponseEnum.ASK)
                //                {
                //                    fileExistsAskEventArgs.SourceFile = sourceFile.FullFileName;
                //                    fileExistsAskEventArgs.DestinationFolder = destinationFolder.FullPath;
                //                    // Publish the event
                //                    currentConflictResponse = EventAggregatorHelper.PublishFileExists(fileExistsAskEventArgs);

                //                    _log.Info($"-- User selected: {currentConflictResponse}");

                //                    if (currentConflictResponse == FileExistsResponseEnum.ASK)
                //                        // This will happen if the event wasn't handled, or wasn't handled correctly
                //                        currentConflictResponse = FileExistsResponseEnum.SKIP;

                //                    if (fileExistsAskEventArgs.Cancel)
                //                    {
                //                        progressInfo.OperationCancelled = true;
                //                        throw new OperationCanceledException(progressInfo.CancellationToken);
                //                    }

                //                    if (fileExistsAskEventArgs.DontAskAgain)
                //                    {
                //                        _log.Info($"   + Dont Ask Again!");
                //                        FileExistsResponse = currentConflictResponse;
                //                    }
                //                }

                //                if (currentConflictResponse == FileExistsResponseEnum.COMPARE)
                //                {
                //                    if (FileSystemHelper.AreSameFiles(sourceFile.FullFileName, destinationFile.GetFullName()))
                //                    {
                //                        _log.Info($"-- Comparison result: Same.");
                //                        currentConflictResponse = FileExistsResponseEnum.SKIP;
                //                    }
                //                    else
                //                    {
                //                        _log.Info($"-- Comparison result: Not same.");
                //                        currentConflictResponse = FileExistsResponseEnum.RENAME;
                //                    }
                //                }

                //                _log.Info($"-- Action: {currentConflictResponse}");

                //                switch (currentConflictResponse)
                //                {
                //                    case FileExistsResponseEnum.OVERWRITE:
                //                        await DoCopy(sourceFile, destinationFile);
                //                        break;
                //                    case FileExistsResponseEnum.SKIP:
                //                        destinationFile.SetStatus(FILE_STATUS.SKIPPED);
                //                        break;
                //                    case FileExistsResponseEnum.RENAME:
                //                        destinationFile.NewName = FileSystemHelper.GetNewFileName(destinationFolder.FullPath, sourceFile.FileName);
                //                        await DoCopy(sourceFile, destinationFile);
                //                        break;
                //                    default:
                //                        break;
                //                }

                //            }
                //            else
                //            {
                //                await DoCopy(sourceFile, destinationFile, false);
                //            }

                //            // report progress
                //            progressInfo.AdvanceWithCancellationToken();
                //        }
                //        catch (OperationCanceledException)
                //        {
                //            throw;
                //        }
                //        catch (Exception ex)
                //        {
                //            destinationFile.SetStatus(FILE_STATUS.ERROR);
                //            destinationFile.Exception = ex;

                //            _errorHandler.Handle(ex, false, $"An error occurred while processing the file: {destinationFile.SourceFile.FileName}");

                //            // Publish the error
                //            // The wrapper can take this event and make a decision whether or not to continue to the next file.
                //            FileErrorEventArgs args = new FileErrorEventArgs(destinationFile);
                //            bool cancel = EventAggregatorHelper.PublishFileError(args);
                //            if (cancel)
                //                throw ex;
                //        }
                //    }
                //}


                #endregion

                #region Old Delete SOurce Files

                //if (Activity.DeleteSourceFiles)
                //{
                //    progressInfo.Text = "Cleaning up...";
                //    var copiedFileList = filesGraph.Files.Values.Where(f => f.Status == FILE_STATUS.COPIED).Select(f => f.FullFileName).ToList();
                //    ShellFileOperation.MoveItemsToRecycleBin(copiedFileList);
                //    progressInfo.Text = "";
                //} 

                #endregion

            }
            finally
            {

            }
        }

        private bool ShouldDeleteSourceFile(SourceFile sourceFile)
        {
            if (!Activity.DeleteSourceFiles)
                return false;

            if (sourceFile.Status == FILE_STATUS.COPIED)
                return true;

            if (sourceFile.Status == FILE_STATUS.SKIPPED && Activity.DeleteSourceFilesOnSkip)
                return true;

            return false;
        }

        private async Task DoCopy(SourceFile sourceFile, DestinationFile destinationFile, bool overwrite = true)
        {
            await Task.Run(() => File.Copy(sourceFile.FullFileName, destinationFile.GetFullName(), overwrite));
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
