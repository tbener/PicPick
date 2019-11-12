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

namespace PicPick.Core
{
    public class Runner
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        public event CopyEventHandler OnCopyStatusChanged;

        private IActivity _activity;
        private IOptions _options;


        public Runner(IActivity activity, IOptions options)
        {
            _activity = activity;
            _options = options;
        }

        public async Task Run(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            _log.Info($"Starting: {_activity.Name}");
            if (_activity.IsRunning) throw new Exception("Activity is already running.");
            EventAggregatorHelper.PublishActivityStarted();

            Analyzer analyzer = new Analyzer(_activity);

            try
            {
                _activity.IsRunning = true;

                // Initialize.Fills the Mapping dictionary
                if (!_activity.Initialized)
                {
                    // When using UI, the Analyze is usually called beforehand, as it gives the initial progress and status information.
                    await analyzer.CreateMapping(progressInfo, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // init progress
                int countTotal = 0;
                foreach (var map in _activity.Mapping.Values)
                {
                    countTotal += map.FileList.Count();
                }
                progressInfo.Maximum = countTotal;
                progressInfo.Value = 0;

                CopyFilesHandler.FileExistsResponse = _options.FileExistsResponse;
                progressInfo.FileExistsResponse = _options.FileExistsResponse;

                // loop on destinations
                foreach (var kv in _activity.Mapping)
                {
                    CopyEventArgs e = new CopyEventArgs(kv.Value);

                    CopyFilesHandler copyFilesHandler = kv.Value;
                    //copyFilesHandler.OnFileProcess += CopyFilesHandler_OnFileProcess;
                    copyFilesHandler.OnFileStatusChanged += CopyFilesHandler_OnFileStatusChanged;
                    string fullPath = PathHelper.GetFullPath(kv.Key, true);
                    copyFilesHandler.SetStart();
                    OnCopyStatusChanged?.Invoke(this, e);

                    progressInfo.MainOperation = $"Copying to {fullPath}";
                    progressInfo.CurrentOperationTotal = copyFilesHandler.FileList.Count();
                    progressInfo.Report();

                    _log.InfoFormat("Copying {0} files to {1}", copyFilesHandler.FileList.Count(), fullPath);

                    // # DO THE ACTUAL COPY
                    await copyFilesHandler.DoCopyAsync(progressInfo, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (_activity.DeleteSourceFiles)
                {
                    progressInfo.MainOperation = "Cleaning up...";
                    var copiedFileList = _activity.FilesInfo.Where(f => f.Value.Status == FILE_STATUS.COPIED).Select(f => f.Key).ToList();
                    Debug.Print($"Moving {copiedFileList.Count()} files to backup ({PathHelper.AppPath("backup")})");
                    string backupPath = PathHelper.GetFullPath(PathHelper.AppPath("backup"), false);
                    ShellFileOperation.DeleteCompletelySilent(backupPath);
                    ShellFileOperation.MoveItems(copiedFileList, PathHelper.GetFullPath(backupPath, true));
                }

            }
            catch (OperationCanceledException)
            {
                _log.Info("The user cancelled the operation");
                progressInfo.UserCancelled = true;
            }
            catch (Exception ex)
            {
                progressInfo.Exception = ex;

                _errorHandler.Handle(ex, false, $"Error in operation: {progressInfo.MainOperation}.");
            }
            finally
            {
                _log.Info($"Finished: {_activity.Name}");
                progressInfo.Finished();
                await Task.Run(() => progressInfo.Report());
                _activity.IsRunning = false;
                _activity.Initialized = false;
                EventAggregatorHelper.PublishActivityEnded();
            }

        }

        private void CopyFilesHandler_OnFileStatusChanged(object sender, string fileFullName, FILE_STATUS status)
        {
            // if status is COPIED we set it only if it didn't fail before
            if (status == FILE_STATUS.COPIED)
                // so if it was set we don't touch it
                if (_activity.FilesInfo[fileFullName].Status != FILE_STATUS.NONE)
                    return;
            _activity.FilesInfo[fileFullName].Status = status;
        }
    }
}
