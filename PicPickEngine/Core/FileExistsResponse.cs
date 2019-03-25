using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class FileExistsResponseAttribute : Attribute
    {
        public string Description { get; set; }
        public string Details { get; set; }

        public static FileExistsResponseAttribute GetAttribute(FileExistsResponseEnum value)
        {
            return ((FileExistsResponseAttribute)GetCustomAttribute(typeof(FileExistsResponseEnum).GetField(value.ToString()), typeof(FileExistsResponseAttribute)));
        }
    }

    public enum FileExistsResponseEnum
    {
        [FileExistsResponse(Description = "Ask me", Details = "Show a dialog and let me decide what to do")]
        ASK,        // Needs to be implemented by the host application
        [FileExistsResponse(Description = "Copy and Replace", Details = "Overwrite the file in the destination folder")]
        OVERWRITE,
        [FileExistsResponse(Description = "Don't copy", Details = "Skip this file (the file will remain in the source folder)")]
        SKIP,
        [FileExistsResponse(Description = "Copy, but keep both files", Details = "The file will be renamed")]
        RENAME,     // Save both
        [FileExistsResponse(Description = "Decide automaically", Details = "Automatically detect whether the images are the same and Skip or Rename accordingly")]
        COMPARE     // Check if same files or just same names. then, if same, skip, if not, keep them both (rename)
    }

    public static class FileExistsResponse
    {

        static FileExistsResponse()
        {
            Dictionary = Enum.GetValues(typeof(FileExistsResponseEnum)).Cast<FileExistsResponseEnum>()
                .ToDictionary(v => v, v => FileExistsResponseAttribute.GetAttribute(v));
        }

        public static Dictionary<FileExistsResponseEnum, FileExistsResponseAttribute> Dictionary { get; private set; }
    }
}
