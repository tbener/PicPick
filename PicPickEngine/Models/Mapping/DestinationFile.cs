using PicPick.Core;
using System;
using System.IO;

namespace PicPick.Models.Mapping
{
    /// <summary>
    /// Represents a file in the destination folder
    /// </summary>
    public class DestinationFile
    {
        private bool? _exists;

        public DestinationFile(SourceFile sourceFile, DestinationFolder destinationFolder)
        {
            SourceFile = sourceFile;
            ParentFolder = destinationFolder;
            // if the folder is new - it will return false, so we don't need to check the file.
            // IMPORTANT: this value might change during the process (as new files might appear). This is why the property is a method with [force] parameter.
            _exists = !(destinationFolder.IsNew || !File.Exists(Path.Combine(destinationFolder.FullPath, sourceFile.FileName)));
        }

        public string GetFullName()
        {
            var fileName = string.IsNullOrEmpty(NewName) ? SourceFile.FileName : NewName;
            return Path.Combine(ParentFolder.FullPath, fileName);
        }

        public void SetStatus(FILE_STATUS status)
        {
            Status = status;
            SourceFile.ReportStatus(status);
        }

        public DestinationFolder ParentFolder { get; set; }
        public SourceFile SourceFile { get; set; }

        /// <summary>
        /// Returns whether the file exists in the destination.
        /// The backing field is initialized in the CTOR but it might be changed during the execution.
        /// For that purpose the forceCheck is actually checking the file.
        /// </summary>
        /// <param name="forceCheck">Force checking if the file actually exists in the destination.</param>
        /// <returns>True if the file exists in the destination.</returns>
        public bool Exists(bool forceCheck = false)
        {
            if (!_exists.HasValue || forceCheck)
                _exists = File.Exists(Path.Combine(ParentFolder.FullPath, SourceFile.FileName));
            return _exists.Value;
        }

        public string NewName { get; set; }
        public FILE_STATUS Status { get; private set; }
        public Exception Exception { get; set; }

        public bool HasError()
        {
            return Exception != null;
        }
    }
}
