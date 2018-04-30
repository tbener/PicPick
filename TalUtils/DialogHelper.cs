using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace TalUtils
{


    public class DialogHelper
    {
        public static bool BrowseOpenFileByExtensions(string[] extensions, bool allowAllFiles, ref string fileNameOrFolder)
        {
            FileDialog dlg = new OpenFileDialog();

            return ShowDialog(dlg, extensions, allowAllFiles, ref fileNameOrFolder);
        }

        public static bool BrowseSaveFileByExtensions(string[] extensions, bool allowAllFiles, ref string fileNameOrFolder)
        {
            FileDialog dlg = new SaveFileDialog();

            return ShowDialog(dlg, extensions, allowAllFiles, ref fileNameOrFolder);
        }

        private static bool ShowDialog(FileDialog dlg, IEnumerable<string> extensions, bool allowAllFiles, ref string fileNameOrFolder)
        {
            dlg.Filter = GetFilter(extensions, allowAllFiles);
            if (fileNameOrFolder.Length > 0)
            {
                if (PathHelper.IsFolder(fileNameOrFolder))
                {
                    dlg.InitialDirectory = fileNameOrFolder;
                }
                else
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(fileNameOrFolder);
                    dlg.FileName = Path.GetFileName(fileNameOrFolder);
                }
            }
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fileNameOrFolder = dlg.FileName;
                return true;
            }
            return false;
        }

        public static bool BrowseOpenFolderDialog(ref string folder)
        {
            var dlg = new FolderBrowser2();
            dlg.DirectoryPath = folder;

            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                folder = dlg.DirectoryPath;
                return true;
            }
            return false;
        }


        private static string ShowDialog1(FileDialog dlg, IEnumerable<string> extensions, bool allowAllFiles, string initialDirectory, string fileName)
        {
            try
            {
                dlg.Filter = GetFilter(extensions, allowAllFiles);
                dlg.InitialDirectory = initialDirectory;
                dlg.FileName = fileName;
                dlg.ShowDialog();
                return dlg.FileName;
            }
            catch (Exception ex)
            {
                Msg.ShowE(ex);
                return string.Empty;
            }

        }


        private static string GetFilter(IEnumerable<string> extensions, bool allowAllFiles)
        {
            string filter = string.Empty;
            string ext;
            foreach (string extension in extensions)
            {
                if (!(ext = extension).Contains("|"))
                    ext = string.Format("{0} files (*.{1})|*.{1}", ext, ext.ToLower());
                filter += ext + "|";
            }
            if (allowAllFiles) filter += "All files (*.*)|*.*";
            else filter = filter.TrimEnd('|');
            return filter;
        }
    }
}
