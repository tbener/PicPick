using PicPick.Core;
using PicPick.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PicPick.Models.Mapping
{
    public class SourceFile
    {
        public SourceFile(string fullPath, bool needDate)
        {
            FullFileName = fullPath;
            FileName = Path.GetFileName(fullPath);
            if (needDate)
            {
                if (!ImageFileInfo.TryGetFileDate(fullPath, out DateTime dateTime))
                    throw new Exception($"Could not extract date from file: {fullPath}");
                DateTime = dateTime;
            }
        }

        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public DateTime DateTime { get; set; }
        public List<DestinationFolder> DestinationFolders { get; set; } = new List<DestinationFolder>();
        public FILE_STATUS Status { get; private set; } = FILE_STATUS.NONE;

        public bool HasError()
        {
            return DestinationFolders.Any(dm => dm.HasError());
        }

        /// <summary>
        /// Source File holds a status that accumulates all its destinations.
        /// The order is: None -> Copied -> Skipped -> Error
        /// The highest status wins.
        /// (e.g. if one destination copied and one skipped, the final status is Skipped).
        /// </summary>
        /// <param name="status"></param>
        internal void ReportStatus(FILE_STATUS status)
        {
            if (Status < status)
                Status = status;
        }
    }
}
