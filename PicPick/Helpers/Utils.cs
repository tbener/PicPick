using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace PicPick.Helpers
{
    class Utils
    {
        /// <summary>
        /// Returns a string representing an error message including all inner exceptions messages
        /// </summary>
        /// <param name="err">Error Exception object</param>
        /// <returns>Error full message</returns>
        public static string GetDetailedErrorMsg(Exception err)
        {
            Exception inner = err;
            StringBuilder msg = new StringBuilder();
            do
            {
                msg.Append(inner.GetType().ToString() + "\r\n" + inner.Message + "\r\nSource: " + inner.Source + "\r\n");
                inner = inner.InnerException;
            } while (inner != null);

            return msg.ToString();
        }

        

        public static string GetUniqueFileName(string directory, string orgFileName)
        {
            string file = orgFileName;
            for (int i = 0; i < 100; i++)
            {
                if (!File.Exists(Path.Combine(directory, file)))
                    return Path.Combine(directory, file);
                file = string.Format("{0}_{2}{1}", Path.GetFileNameWithoutExtension(orgFileName), Path.GetExtension(orgFileName), i.ToString("00"));
            }
            return "";
        }

        /// <summary>
        /// Loads/Creates an object instance
        /// </summary>
        /// <param name="filePath">The path to the assembly to load</param>
        /// <param name="type">Type to be created</param>
        /// <returns>The newly created object, or null if an error occurres</returns>
        public static object CreateLateBoundObjectFromFile(string filePath)
        {
            object returnObject = null;

            try
            {
                Assembly asm = Assembly.LoadFrom(filePath);
                Type[] t = asm.GetTypes();
                returnObject = asm.CreateInstance(t[0].FullName);
            }
            catch 
            {
                returnObject = null;
            }

            return returnObject;
        }

        public static object CreateLateBoundObjectFromFile(string assemblyName, string typeName)
        {
            object returnObject = null;

            try
            {
                return Activator.CreateInstance(assemblyName, typeName);

            }
            catch 
            {
                returnObject = null;
            }

            return returnObject;
        }

        

        #region Windows Explorer

        public static void OpenFile(string fileName)
        {
            OpenFile(fileName, "open");
        }
        public static void OpenFile(string fileName, string command)
        {
            OpenFile(fileName, "open", "");
        }
        public static void OpenFile(string fileName, string command, string args)
        {
            Process p = new Process();
            p.StartInfo.Verb = command;
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = args;
            p.Start();
        }

        public static void OpenPath(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Explorer.exe";
            p.StartInfo.Arguments = path;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();
        }

        public static void OpenContainingFolder(string fileName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Explorer.exe";
            p.StartInfo.Arguments = "/Select," + fileName;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();
        }

        #endregion
    }

    
}
