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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

[assembly:InternalsVisibleTo("PicPick.UnitTests")]
namespace PicPick.Core
{
    /// <summary>
    /// </summary>
    public class Mapper : IAction
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        // this has the same structure of the main FilesGraph, only it doesn't include the final filter
        private List<SourceFile> _sourceFiles = new List<SourceFile>();
        private Dictionary<string, DestinationFolder> _destinationFoldersDictionary = new Dictionary<string, DestinationFolder>();
        private List<PicPickProjectActivityDestination> _destinations;

        public PicPickProjectActivity Activity { get; set; }

        public Mapper(IActivity activity)
        {
            Activity = (PicPickProjectActivity)activity;
            Activity.FileGraph.SetMapper(this);
        }

        internal void Clear()
        {
            _sourceFiles.Clear();
            _destinationFoldersDictionary.Clear();
            _destinationFolders = null;
        }

        #region Public Methods

        internal async Task MapAsync(IProgressInformation progressInfo)
        {
            _destinations = Activity.DestinationList.Where(d => d.Active).ToList();

            bool needDates = _destinations.Any(d => d.HasTemplate);
            needDates = needDates || Activity.Source.FromDate.Use || Activity.Source.ToDate.Use;

            List<SourceFile> sourceFilesList = await ReadFileListToObjects(progressInfo, needDates);

            ApplyFirstFilters(sourceFilesList);

            CreateDestinationObjects();
        }

        /// <summary>
        /// This is the last function to be called in the mapping process,
        /// and it is the only one which considers the OnlyNewFiles filter.
        /// </summary>
        internal void ApplyFinalFilters()
        {
            _destinationFolders = null;
            if (Activity.Source.OnlyNewFiles)
            {
                Activity.FileGraph.Files = _sourceFiles.Where(sf => !sf.ExistsInDestination).ToList();
            }
            else
            {
                Activity.FileGraph.Files = _sourceFiles.ToList();
            }
        }

        private List<DestinationFolder> _destinationFolders;

        internal List<DestinationFolder> GetDestinationFolders()
        {
            if (_destinationFolders == null)
            {
                if (Activity.Source.OnlyNewFiles)
                {
                    _destinationFolders = new List<DestinationFolder>();
                    foreach (SourceFile sourceFile in Activity.FileGraph.Files)
                    {
                        foreach (DestinationFolder df in sourceFile.DestinationFolders)
                        {
                            if (!_destinationFolders.Contains(df))
                                _destinationFolders.Add(df);
                        }
                    }
                }
                else
                {
                    _destinationFolders = _destinationFoldersDictionary.Values.ToList();
                }
            }

            return _destinationFolders;
        }

        #endregion

        #region Computing and mapping methods

        private async Task<List<SourceFile>> ReadFileListToObjects(IProgressInformation progressInfo, bool needDates)
        {
            List<SourceFile> sourceFilesList = new List<SourceFile>();

            Clear();

            progressInfo.Maximum = Activity.FileGraph.RawFileList.Count(); // source.FileList.Count();
            progressInfo.Value = 0;

            // Initiate the instances of the SourceFiles class
            await Task.Run(() =>
            {
                foreach (string files in Activity.FileGraph.RawFileList)
                {
                    sourceFilesList.Add(new SourceFile(files, needDates));
                    progressInfo.AdvanceWithCancellationToken();
                }
            });

            return sourceFilesList;
        }

        private void ApplyFirstFilters(List<SourceFile> sourceFilesList)
        {
            // Add Source Files to the real list
            if (Activity.Source.FromDate.Use || Activity.Source.ToDate.Use)
            {
                DateTime fromDate = Activity.Source.FromDate.Use ? Activity.Source.FromDate.Date : DateTime.MinValue;
                DateTime toDate = Activity.Source.ToDate.Use ? Activity.Source.ToDate.Date : DateTime.MaxValue;
                foreach (SourceFile sf in sourceFilesList)
                {
                    if (sf.DateTime >= fromDate && sf.DateTime <= toDate)
                        _sourceFiles.Add(sf);
                }
            }
            else
                _sourceFiles.AddRange(sourceFilesList);

        }

        private void CreateDestinationObjects()
        {
            // the following loop should be very quick
            // no need for progress update
            foreach (PicPickProjectActivityDestination destination in _destinations)
            {
                if (destination.HasTemplate)
                {
                    foreach (SourceFile sourceFile in _sourceFiles)
                    {
                        var destinationFullPath = destination.GetFullPath(sourceFile.DateTime);

                        if (!_destinationFoldersDictionary.TryGetValue(destinationFullPath, out DestinationFolder destinationFolder))
                        {
                            destinationFolder = new DestinationFolder(destinationFullPath, destination, Activity);
                            _destinationFoldersDictionary.Add(destinationFullPath, destinationFolder);
                        }
                        // This will do both adding a reference from the SourceFile to the destinationFolder and adding a new DestinationFile object to this destinationFolder
                        destinationFolder.AddFile(sourceFile);
                    }
                }
                else
                {
                    // it will be a single DestinationFolder for all files
                    DestinationFolder destinationFolder = new DestinationFolder(destination.GetFullPath(), destination, Activity);
                    _destinationFoldersDictionary.Add(destinationFolder.FullPath, destinationFolder);
                    _sourceFiles.ForEach(destinationFolder.AddFile);
                }
            }
        }


        

        #endregion

        #region Public Properties


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("----- Plan Start -----\n");

            sb.AppendLine($"Files count: {_sourceFiles.Count}");
            sb.AppendLine($"Destination folders count: {Activity.FileGraph.DestinationFolders.Count}");

            sb.AppendLine("\nActive Destinations:");
            foreach (PicPickProjectActivityDestination dest in _destinations)
            {
                sb.AppendLine($"{dest.GetFullPath()}\\[{dest.Template}]");
            }

            sb.AppendLine();

            foreach (DestinationFolder destination in Activity.FileGraph.DestinationFolders)
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
