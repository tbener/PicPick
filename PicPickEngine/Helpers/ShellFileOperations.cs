﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PicPick.Helpers
{
    class ShellFileOperation
    {

        /// <summary>
        /// Possible flags for the SHFileOperation method.
        /// </summary>
        [Flags]
        public enum FileOperationFlags : ushort
        {
            /// <summary>
            /// Do not show a dialog during the process
            /// </summary>
            FOF_SILENT = 0x0004,
            /// <summary>
            /// Do not ask the user to confirm selection
            /// </summary>
            FOF_NOCONFIRMATION = 0x0010,
            /// <summary>
            /// Delete the file to the recycle bin.  (Required flag to send a file to the bin
            /// </summary>
            FOF_ALLOWUNDO = 0x0040,
            /// <summary>
            /// Do not show the names of the files or folders that are being recycled.
            /// </summary>
            FOF_SIMPLEPROGRESS = 0x0100,
            /// <summary>
            /// Surpress errors, if any occur during the process.
            /// </summary>
            FOF_NOERRORUI = 0x0400,
            /// <summary>
            /// Warn if files are too big to fit in the recycle bin and will need
            /// to be deleted completely.
            /// </summary>
            FOF_WANTNUKEWARNING = 0x4000,
        }

        /// <summary>
        /// File Operation Function Type for SHFileOperation
        /// </summary>
        public enum FileOperationType : uint
        {
            /// <summary>
            /// Move the objects
            /// </summary>
            FO_MOVE = 0x0001,
            /// <summary>
            /// Copy the objects
            /// </summary>
            FO_COPY = 0x0002,
            /// <summary>
            /// Delete (or recycle) the objects
            /// </summary>
            FO_DELETE = 0x0003,
            /// <summary>
            /// Rename the object(s)
            /// </summary>
            FO_RENAME = 0x0004,
        }



        /// <summary>
        /// SHFILEOPSTRUCT for SHFileOperation from COM
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        private struct SHFILEOPSTRUCT
        {

            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public FileOperationType wFunc;
            public string pFrom;
            public string pTo;
            public FileOperationFlags fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// Send file to recycle bin
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        /// <param name="flags">FileOperationFlags to add in addition to FOF_ALLOWUNDO</param>
        public static bool Send(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = FileOperationFlags.FOF_ALLOWUNDO | flags
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send file to recycle bin.  Display dialog, display warning if files are too big to fit (FOF_WANTNUKEWARNING)
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool Send(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_WANTNUKEWARNING);
        }

        /// <summary>
        /// Send file silently to recycle bin.  Surpress dialog, surpress errors, delete if too large.
        /// </summary>
        /// <param name="path">Location of directory or file to recycle</param>
        public static bool MoveToRecycleBin(string path)
        {
            return Send(path, FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI | FileOperationFlags.FOF_SILENT);

        }

        private static bool deleteFile(string path, FileOperationFlags flags)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_DELETE,
                    pFrom = path + '\0' + '\0',
                    fFlags = flags
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CopyItems(List<string> items, string destination)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_COPY,
                    pFrom = string.Join("\0", items.ToArray()) + '\0' + '\0',
                    pTo = destination + '\0' + '\0'
                };

                // tbener 3/2018 - changed to return False if cancelled by user
                int res = SHFileOperation(ref fs);
                if (res != 0)
                    Debug.Print($"RESULT RETURNED: {res}");

                return res == 0;
            }
            catch (Exception)
            {
                // tbener 3/2018 - change from return false (as opposed to cancel)
                throw;
            }
        }

        public static bool MoveItems(List<string> items, string destination)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = FileOperationType.FO_MOVE,
                    pFrom = string.Join("\0", items.ToArray()) + '\0' + '\0',
                    pTo = destination + '\0' + '\0'
                };
                // tbener 3/2018 - changed to return False if cancelled by user
                return SHFileOperation(ref fs) == 0;
            }
            catch (Exception)
            {
                // tbener 3/2018 - change from return false (as opposed to cancel)
                throw;
            }
        }

        // Main overload for standard Move\Copy file(s) operation
        private static bool DoFileOperation(FileOperationType foType, string[] files, string destination)
        {
            try
            {
                var fs = new SHFILEOPSTRUCT
                {
                    wFunc = foType,
                    pFrom = string.Join("\0", files) + '\0' + '\0',
                    pTo = destination + '\0' + '\0'
                };
                SHFileOperation(ref fs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool DoFileOperation(FileOperationType foType, string file, string destination)
        {
            return DoFileOperation(foType, new[] {file}, destination);
        }

        public static bool Copy(string source, string destination)
        {
            return DoFileOperation(FileOperationType.FO_COPY, source, destination);
        }

        public static bool Move(string source, string destination)
        {
            return DoFileOperation(FileOperationType.FO_COPY, source, destination);
        }

        public static bool DeleteCompletelySilent(string path)
        {
            return deleteFile(path,
                              FileOperationFlags.FOF_NOCONFIRMATION | FileOperationFlags.FOF_NOERRORUI |
                              FileOperationFlags.FOF_SILENT);
        }
    }
}