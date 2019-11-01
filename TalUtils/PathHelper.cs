﻿using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TalUtils
{
    public class PathHelper
    {
        #region Path Methods

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        public static string ShortDisplay(string path, int length)
        {
            StringBuilder sb = new StringBuilder(length);
            PathCompactPathEx(sb, path, length, 0);
            return sb.ToString();
        }

        public static string AppPath()
        {
            return AppPath("");
        }
        public static string AppPath(string join)
        {
            try
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), join);
            }
            catch
            {
                return ExecutionPath(join);
            }
        }

        public static string ExecutionPath()
        {
            return ExecutionPath("");
        }
        public static string ExecutionPath(string join)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), join);
        }

        public static bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public static bool Exists(string root, string path)
        {
            return Exists(GetFullPath(root, path));
        }

        public static string GetFullPath(string path)
        {
            return GetFullPath(AppPath(), path, false);
        }

        public static string GetFullPath(string root, string path)
        {
            return GetFullPath(root, path, false);
        }

        public static string GetFullPath(string path, bool buildPath)
        {
            return GetFullPath(AppPath(), path, buildPath);
        }

        /// <summary>
        /// Main Overload.
        /// Build full path combined from root + path
        /// If path is not a relative path then return it as is
        /// </summary>
        /// <param name="root">The beginning of the path in case [path] is not rooted</param>
        /// <param name="path">The end part of the path</param>
        /// <param name="buildPath">If True - create the path if not exists</param>
        /// <returns>string - full rooted path</returns>
        public static string GetFullPath(string root, string path, bool buildPath)
        {
            string fullPath = path;
            if (!Path.IsPathRooted(path)) fullPath = Path.Combine(root, path);
            if (buildPath)
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            return fullPath;
        }

        #endregion

        /// <summary>
        /// Returns true if the given file path is a folder.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>True if a folder</returns>
        public static bool IsFolder(string path)
        {
            try
            {
                return ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory);
            }
            catch
            {
                var extension = Path.GetExtension(path);
                return extension != null && extension.Length == 0;
            }
        }

        /// <summary>
        /// if relativeTo = "C:\aaa\bbb\ccc"
        /// and fullPathh = "C:\aaa\bbb\ccc\ddd"
        /// return "ddd"
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetRelativePath(string relativeTo, string fullPath)
        {
            if (fullPath.StartsWith(relativeTo))
                return fullPath.Substring(relativeTo.Length).TrimStart('\\');

            return fullPath;
        }
    }
}
