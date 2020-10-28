using log4net;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PicPick.UnitTests")]
namespace PicPick.Core
{
    public class Reader : IAction
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PicPickProjectActivity Activity { get; set; }

        public Reader(IActivity activity)
        {
            Activity = (PicPickProjectActivity)activity;
        }

        public Reader()
        {
        }

        internal bool ReadFiles()
        {
            _log.Info("Reading files...");
            try
            {
                Activity.FileGraph.Clear();

                var source = Activity.Source;

                List<string> lstFiles = new List<string>();
                string[] filters = source.Filter.Replace(" ", "").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                SearchOption searchOption = source.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                if (filters.Count() == 0 || filters.Contains("*.*"))
                    filters = new[] { "*.*" };

                _log.Info($"-- Path: {source.Path}, filters: [{source.Filter}], search options: {searchOption}");

                // loop on filters
                foreach (string fltr in filters)
                {
                    string filter = fltr.Trim();
                    _log.Info($"-- Reading {filter}");
                    // get file list for current filter
                    string[] fileEntries = Directory.GetFiles(source.Path, filter, searchOption);
                    _log.Info($"---- Found {fileEntries.Length}");
                    // add to main file list (could include duplicates)
                    lstFiles.AddRange(fileEntries);
                }

                // create a unique file list
                Activity.FileGraph.RawFileList = new HashSet<string>(lstFiles);
                return true;
            }
            catch (DirectoryNotFoundException ex) {
                _log.Error($"-- {ex.Message}");
                return false;
            }
        }
    }
}
