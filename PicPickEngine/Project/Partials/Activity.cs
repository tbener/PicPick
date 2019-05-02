using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TalUtils;
using PicPick.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PicPick.Core;

namespace PicPick.Project
{

    /// <summary>
    /// This class extends PicPickProjectActivity and includes:
    /// 1. Extension properties
    /// 2. Execution methods
    /// </summary>
    public partial class PicPickProjectActivity : ICloneable
    {

        public event CopyEventHandler OnCopyStatusChanged;

        private ObservableCollection<PicPickProjectActivityDestination> _destinationList = null;

        private Dictionary<string, CopyFilesHandler> _mapping = new Dictionary<string, CopyFilesHandler>();
        private Dictionary<string, PicPickFileInfo> _dicFiles = new Dictionary<string, PicPickFileInfo>();
        private List<string> _errorFiles = new List<string>();

        public PicPickProjectActivity(string name)
        {
            Name = name;
            Source = new PicPickProjectActivitySource();
        }

        

        /// <summary>
        /// Use this list rather than the Destination Array for easyer manipulations and editing.
        /// This will be converted back to the Destination Array in ConfigurationHelper.Save()
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<PicPickProjectActivityDestination> DestinationList
        {
            get
            {
                if (_destinationList == null)
                {
                    if (this.Destination == null)
                        this.Destination = new PicPickProjectActivityDestination[0];
                    _destinationList = new ObservableCollection<PicPickProjectActivityDestination>();
                    
                    foreach (PicPickProjectActivityDestination dest in this.Destination)
                    {
                        _destinationList.Add(dest);
                    }
                }
                return _destinationList;
            }
        }



        [XmlIgnore]
        public bool Initialized { get; private set; }

        [XmlIgnore]
        public FileExistsResponseEnum FileExistsResponse { get; set; }


        #region Execution



        /*
         * Lists the files and their dates
         */
        private async Task ReadFilesDatesAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {

            DateTime dateTime = DateTime.MinValue;
            ImageFileInfo fileDateInfo = new ImageFileInfo();

            _dicFiles.Clear();
            _errorFiles.Clear();

            progressInfo.CurrentOperation = "Reading files dates";
            progressInfo.Maximum = Source.FileList.Count;
            progressInfo.Start();

            foreach (string file in Source.FileList)
            {
                if (fileDateInfo.GetFileDate(file, out dateTime))
                    _dicFiles.Add(file, new PicPickFileInfo(dateTime));
                else
                    _errorFiles.Add(file);
                await Task.Run(() => progressInfo.Advance());
                cancellationToken.ThrowIfCancellationRequested();
            }

            Debug.Print($"Found {_dicFiles.Count()} files");
            if (_errorFiles.Count() > 0)
            {
                Debug.Print($"ERRORS: Couldn't get dates from {_errorFiles.Count()} files:");
                foreach (string file in _errorFiles)
                {
                    Debug.Print($"\t{file}");
                }
            }

        }

        /// <summary>
        /// Create the Mapping structure.
        /// the Mapping is a list of CopyFilesHandler objects.
        /// every CopyFilesHandler object holds a list of files that should be copied to a single folder.
        /// This destination folder also used as the Mapping key.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Analyze(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            progressInfo.MainOperation = "Analyzing...";
            _mapping.Clear();
            await ReadFilesDatesAsync(progressInfo, cancellationToken);

            progressInfo.CurrentOperation = "Mapping files to destinations";
            
            var activeDestinations = DestinationList.Where(d => d.Active).ToList();
            progressInfo.Maximum = _dicFiles.Count * activeDestinations.Count;
            progressInfo.Start();

            foreach (PicPickProjectActivityDestination destination in activeDestinations)
            {
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
                        await Task.Run(() => progressInfo.Advance());
                    }
                else
                {
                    if (!_mapping.ContainsKey(pathAbsolute))
                    {
                        _mapping.Add(pathAbsolute, new CopyFilesHandler(pathAbsolute));
                    }
                    _mapping[pathAbsolute].AddRange(_dicFiles.Keys.ToList());
                    await Task.Run(() => progressInfo.Advance(_dicFiles.Count));
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

            progressInfo.Done = true;
            progressInfo.MainOperation = "Finished Analyzing";

            Initialized = true;
            return true;
        }

        [XmlIgnore]
        public bool IsRunning { get; set; }

        /// <summary>
        /// Executes the whole task. Copying the files to ALL destinations
        /// </summary>
        /// <param name="progressInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Start(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            IsRunning = true;
            EventAggregatorHelper.PublishActivityStarted();

            progressInfo.Activity = Name;

            // Initialize.Fills the Mapping dictionary
            if (!Initialized)
            {
                // When using UI, the Analyze is usually called beforehand, as it gives the initial progress and status information.
                await Analyze(progressInfo, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                // init progress
                int countTotal = 0;
                foreach (var map in _mapping.Values)
                {
                    countTotal += map.FileList.Count();
                }
                progressInfo.Maximum = countTotal;
                progressInfo.Value = 0;

                CopyFilesHandler.FileExistsResponse = FileExistsResponse;
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

                    // # DO THE ACTUAL COPY
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
                Initialized = false;
                progressInfo.Done = true;
                await Task.Run(() => progressInfo.Report());
                IsRunning = false;
                EventAggregatorHelper.PublishActivityEnded();
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

       


        #endregion

        #region ICloneable

        public object Clone()
        {
            PicPickProjectActivity newTask = new PicPickProjectActivity()
            {
                Name = Name,
                DeleteSourceFiles = DeleteSourceFiles
            };
            if (Source != null)
                newTask.Source = new PicPickProjectActivitySource()
                {
                    Path = Source.Path,
                    Filter = Source.Filter
                };

            if (Destination != null)
                newTask.Destination = (PicPickProjectActivityDestination[])Destination.Clone();

            return newTask;

        }

        #endregion
    }

}
