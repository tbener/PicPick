using log4net;
using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.Models.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Core
{
    /// <summary>
    /// This class is a dictionary that 
    /// - its Key is the Source File path
    /// - its Value is a SourceFile class
    /// Every SourceFile class splits into its destinations, and then to the destination files.
    /// 
    /// These classes are updated during the analyzing process and then during the running porcess itself.
    /// </summary>
    public class Mapper : IAction
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        // this has the same structure of the main FilesGraph, only it doesn't include the final filter
        private Dictionary<string, SourceFile> _sourceFilesDictionary = new Dictionary<string, SourceFile>();
        private List<PicPickProjectActivityDestination> _destinations;

        public IActivity Activity { get; set; }

        public Mapper(IActivity activity)
        {
            Activity = activity;
        }

        internal void Clear()
        {
            _sourceFilesDictionary.Clear();
            Activity.FilesGraph.DestinationFolders.Clear();
        }

        #region ComputeAsync

        public async Task MapAsync(ProgressInformation progressInfo)
        {
            _destinations = Activity.DestinationList.Where(d => d.Active).ToList();

            bool needDates = _destinations.Any(d => d.HasTemplate);
            needDates = needDates || Activity.Source.FromDate.Use || Activity.Source.ToDate.Use;

            List<SourceFile> sourceFilesList = await ReadFileListToObjects(progressInfo, needDates);

            FilterIntoDictionary(sourceFilesList);

            CreateDestinationObjects();
        }

        //public async Task ComputeAsync(ProgressInformation progressInfo, int levelUpdate = 0)
        //{
        //    Destinations = Activity.DestinationList.Where(d => d.Active).ToList();

        //    bool needDates = Destinations.Any(d => d.HasTemplate);
        //    needDates = needDates || Activity.Source.FromDate.Use || Activity.Source.ToDate.Use;

        //    if (levelUpdate < 3)
        //    {
        //        // level 0
        //        List<SourceFile> sourceFilesList = await ReadFileListToObjects(progressInfo, needDates);

        //        // level 1
        //        FilterIntoDictionary(sourceFilesList);

        //        CreateDestinationObjects();
        //    }

        //    // level 3
        //    ApplyFinalFilters();

        //    _log.Info("Plan is:\n" + ToString());
        //}

        #endregion

        #region Computing and mapping methods

        private async Task<List<SourceFile>> ReadFileListToObjects(ProgressInformation progressInfo, bool needDates)
        {
            List<SourceFile> sourceFilesList = new List<SourceFile>();

            Clear();
            ValidateFields();

            progressInfo.Maximum = Activity.FilesGraph.RawFileList.Count(); // source.FileList.Count();
            progressInfo.Value = 0;

            // Initiate the instances of the SourceFiles class
            await Task.Run(() =>
            {
                foreach (string files in Activity.FilesGraph.RawFileList)
                {
                    sourceFilesList.Add(new SourceFile(files, needDates));
                    progressInfo.AdvanceWithCancellationToken();
                }
            });

            return sourceFilesList;
        }

        private void FilterIntoDictionary(List<SourceFile> sourceFilesList)
        {
            // Add Source Files to dictionary
            if (Activity.Source.FromDate.Use || Activity.Source.ToDate.Use)
            {
                DateTime fromDate = Activity.Source.FromDate.Use ? Activity.Source.FromDate.Date : DateTime.MinValue;
                DateTime toDate = Activity.Source.ToDate.Use ? Activity.Source.ToDate.Date : DateTime.MaxValue;
                foreach (SourceFile sf in sourceFilesList)
                {
                    if (sf.DateTime >= fromDate && sf.DateTime <= toDate)
                        _sourceFilesDictionary.Add(sf.FullFileName, sf);
                }
            }
            else
                sourceFilesList.ForEach(sf => _sourceFilesDictionary.Add(sf.FullFileName, sf));

        }

        private void CreateDestinationObjects()
        {
            // the following loop should be very quick
            // no need for progress update
            foreach (PicPickProjectActivityDestination destination in _destinations)
            {
                if (destination.HasTemplate)
                {
                    foreach (SourceFile sourceFile in _sourceFilesDictionary.Values)
                    {
                        var destinationFullPath = destination.GetFullPath(sourceFile.DateTime);

                        if (!Activity.FilesGraph.DestinationFolders.TryGetValue(destinationFullPath, out DestinationFolder destinationFolder))
                        {
                            destinationFolder = new DestinationFolder(destinationFullPath, destination);
                            Activity.FilesGraph.DestinationFolders.Add(destinationFullPath, destinationFolder);
                        }
                        // This will do both adding a reference from the SourceFile to the destinationFolder and adding a new DestinationFile object to this destinationFolder
                        destinationFolder.AddFile(sourceFile);
                    }
                }
                else
                {
                    // it will be a single DestinationFolder for all files
                    DestinationFolder destinationFolder = new DestinationFolder(destination.Path, destination);
                    Activity.FilesGraph.DestinationFolders.Add(destinationFolder.FullPath, destinationFolder);
                    _sourceFilesDictionary.Values.ToList().ForEach(destinationFolder.AddFile);
                }
            }
        }


        /// <summary>
        /// This is the last function to be called in the mapping process,
        /// and it is the only one which considers the OnlyNewFiles filter.
        /// </summary>
        public void ApplyFinalFilters()
        {
            if (Activity.Source.OnlyNewFiles)
                Activity.FilesGraph.Files = _sourceFilesDictionary.Where(sf => !sf.Value.ExistsInDestination).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            else
                Activity.FilesGraph.Files = _sourceFilesDictionary;
        }

        #endregion

        #region More Public Methods

        public void ValidateFields()
        {
            string realPath;
            bool isRealTemplate;

            if (Activity.Source == null || Activity.Source.Path == "")
                throw new NoSourceException();

            if (!Directory.Exists(Activity.Source.Path))
                throw new SourceDirectoryNotFoundException();

            if (Activity.DestinationList.Count == 0)
                throw new NoDestinationsException();

            foreach (var dest in Activity.DestinationList.Where(d => d.Active))
            {
                realPath = dest.GetTemplatePath(DateTime.Now);
                isRealTemplate = !realPath.Equals(dest.Template);
                if (isRealTemplate)
                    continue;

                realPath = Path.Combine(PathHelper.GetFullPath(Activity.Source.Path, dest.Path), dest.Template);

                if (realPath.Equals(Activity.Source.Path, StringComparison.OrdinalIgnoreCase))
                    throw new DestinationEqualsSourceException();
            }
        }

        #endregion

        #region Public Properties


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("----- Plan Start -----\n");

            sb.AppendLine($"Files count: {_sourceFilesDictionary.Count}");
            sb.AppendLine($"Destination folders count: {Activity.FilesGraph.DestinationFolders.Count}");

            sb.AppendLine("\nActive Destinations:");
            foreach (PicPickProjectActivityDestination dest in _destinations)
            {
                sb.AppendLine($"{dest.Path}\\[{dest.Template}]");
            }

            sb.AppendLine();

            foreach (DestinationFolder destination in Activity.FilesGraph.DestinationFolders.Values)
            {
                sb.AppendLine(string.Format("{0} ({1})", destination.FullPath, destination.IsNew ? "New" : "Exists"));
                foreach (DestinationFile file in destination.Files)
                {
                    sb.AppendLine($"--> {file.GetFullName()}");
                }
            }

            sb.AppendLine("----- Plan End -----");

            return sb.ToString();
        }

        #endregion

    }
}
