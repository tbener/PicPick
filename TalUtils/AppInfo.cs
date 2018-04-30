using System;
using System.IO;
using System.Reflection;

namespace TalUtils
{
    public static class AppInfo
    {
        public static string AppPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        public static string AppExeFullPath
        {
            get
            {
                return Assembly.GetEntryAssembly().Location;
            }
        }

        public static string AppName
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        public static Version AppVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        public static string AppVersionString
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString(3);
            }
        }

        public static string AppVersionFull(string format = "{0} (revision {1})")
        {
            return string.Format(format, Assembly.GetEntryAssembly().GetName().Version.ToString(3), AppRevisionNumber);
        }

        public static int AppBuildNumber
        {
            get { return Assembly.GetEntryAssembly().GetName().Version.Build; }
        }

        public static int AppRevisionNumber
        {
            get { return Assembly.GetEntryAssembly().GetName().Version.Revision; }
        }

        public static string AppDescription
        {
            get
            {
                return ((AssemblyDescriptionAttribute)Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public static string Author
        {
            get
            {
                return "Tal Bener";
            }
        }
    }
}
