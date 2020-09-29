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
        public IActivity Activity { get; set; }

        public Reader(IActivity activity)
        {
            Activity = activity;
        }

        public Reader()
        {
        }

        internal void ReadFiles()
        {
            Activity.FilesGraph.Clear();

            var source = Activity.Source;

            List<string> lstFiles = new List<string>();
            string[] filters = source.Filter.Replace(" ", "").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            SearchOption searchOption = source.IncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            if (filters.Count() == 0 || filters.Contains("*.*"))
                filters = new[] { "*.*" };

            // loop on filters
            foreach (string fltr in filters)
            {
                string filter = fltr.Trim();
                // get file list for current filter
                string[] fileEntries = Directory.GetFiles(source.Path, filter, searchOption);
                // add to main file list (could include duplicates)
                lstFiles.AddRange(fileEntries);
            }

            // create a unique file list
            Activity.FilesGraph.RawFileList = new HashSet<string>(lstFiles);
        }
    }
}
