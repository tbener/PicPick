using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.Core
{
    public class Analyzer
    {
        private IActivity _activity;
        private List<string> _filesError = new List<string>();

        public Analyzer(IActivity activity)
        {
            _activity = activity;
            if (Mapping == null) Mapping = new Dictionary<string, CopyFilesHandler>();
            if (FilesInfo == null) FilesInfo = new Dictionary<string, PicPickFileInfo>();
        }

        public bool MappingCompletedSuccessfully { get; private set; }

        public Dictionary<string, CopyFilesHandler> Mapping { get => _activity.Mapping; private set => _activity.Mapping = value; }
        public Dictionary<string, PicPickFileInfo> FilesInfo { get => _activity.FilesInfo; private set => _activity.FilesInfo = value; }

        /// <summary>
        /// Create the Mapping structure.
        /// the Mapping is a list of CopyFilesHandler objects.
        /// every CopyFilesHandler object holds a list of files that should be copied to a single folder.
        /// This destination folder also used as the Mapping key.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CreateMapping(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {
            progressInfo.MainOperation = "Analyzing...";
            MappingCompletedSuccessfully = false;
            Mapping.Clear();

            ValidateFields();

            await ReadFilesDatesAsync(progressInfo, cancellationToken);

            progressInfo.CurrentOperation = "Mapping files to destinations";

            var activeDestinations = _activity.DestinationList.Where(d => d.Active).ToList();
            progressInfo.Maximum = FilesInfo.Count * activeDestinations.Count;
            progressInfo.Start();

            foreach (PicPickProjectActivityDestination destination in activeDestinations)
            {
                string pathAbsolute = PathHelper.GetFullPath(_activity.Source.Path, destination.Path);
                if (destination.HasTemplate)
                    foreach (var kv in FilesInfo)
                    {
                        // create the string from template
                        string relPath = destination.GetTemplatePath(kv.Value.DateTime);
                        string fullPath = PathHelper.GetFullPath(pathAbsolute, relPath);
                        // add to mapping dictionary
                        if (!Mapping.ContainsKey(fullPath))
                        {
                            // new destination path
                            Mapping.Add(fullPath, new CopyFilesHandler(fullPath));
                        }
                        Mapping[fullPath].AddFile(kv.Key);
                        await Task.Run(() => progressInfo.Advance());
                    }
                else
                {
                    if (!Mapping.ContainsKey(pathAbsolute))
                    {
                        Mapping.Add(pathAbsolute, new CopyFilesHandler(pathAbsolute));
                    }
                    Mapping[pathAbsolute].AddRange(FilesInfo.Keys.ToList());
                    await Task.Run(() => progressInfo.Advance(FilesInfo.Count));
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

            MappingCompletedSuccessfully = true;
            _activity.Initialized = true;
            progressInfo.MainOperation = "Finished Analyzing";

        }

        /*
         * Lists the files and their dates
         */
        private async Task ReadFilesDatesAsync(ProgressInformation progressInfo, CancellationToken cancellationToken)
        {

            DateTime dateTime = DateTime.MinValue;
            ImageFileInfo fileDateInfo = new ImageFileInfo();

            FilesInfo.Clear();
            _filesError.Clear();

            progressInfo.CurrentOperation = "Reading files dates";
            progressInfo.Maximum = _activity.Source.FileList.Count;
            progressInfo.Start();

            foreach (string file in _activity.Source.FileList)
            {
                if (fileDateInfo.GetFileDate(file, out dateTime))
                    FilesInfo.Add(file, new PicPickFileInfo(dateTime));
                else
                    _filesError.Add(file);
                await Task.Run(() => progressInfo.Advance());
                cancellationToken.ThrowIfCancellationRequested();
            }

            Debug.Print($"Found {FilesInfo.Count()} files");
            if (_filesError.Count() > 0)
            {
                Debug.Print($"ERRORS: Couldn't get dates from {_filesError.Count()} files:");
                foreach (string file in _filesError)
                {
                    Debug.Print($"\t{file}");
                }
            }

        }

        public void ValidateFields()
        {
            string realPath;
            bool isRealTemplate;

            if (_activity.Source == null || _activity.Source.Path == "")
                throw new NoSourceException();

            if (!Directory.Exists(_activity.Source.Path))
                throw new SourceDirectoryNotFoundException();

            if (_activity.DestinationList.Count == 0)
                throw new NoDestinationsException();

            foreach (var dest in _activity.DestinationList.Where(d => d.Active))
            {
                realPath = dest.GetTemplatePath(DateTime.Now);
                isRealTemplate = !realPath.Equals(dest.Template);
                if (isRealTemplate)
                    continue;

                realPath = Path.Combine(PathHelper.GetFullPath(_activity.Source.Path, dest.Path), dest.Template);

                if (realPath.Equals(_activity.Source.Path, StringComparison.OrdinalIgnoreCase))
                    throw new DestinationEqualsSourceException();
            }
        }
    }
}
