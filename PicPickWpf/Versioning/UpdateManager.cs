using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.Properties;
using DiGit.Versioning;
using DiGit.View;

namespace DiGit.Versioning
{
    public delegate void UpdateInfoChangedEventHandler(object sender, EventArgs arg);
    public delegate void UpdateRequiredEventHandler(object sender, EventArgs arg);

    internal static class UpdateManager
    {
        private static Exception _lastReadError;
        public static event UpdateInfoChangedEventHandler OnUpdateInfoChanged;
        public static event UpdateRequiredEventHandler OnUpdateRequired;
        public static DiGitVersionInfo VersionInfo { get; private set; }


        internal static void CheckRemoteAsync()
        {
            Task.Factory.StartNew(CheckRemote);
        }

        internal static void CheckRemote()
        {
            if (Working) return;
            Working = true;
            UserManager.AddLog("Check Update", "Start");
            try
            {
                LastReadError = null;
                OnUpdateInfoChanged?.Invoke(null, new EventArgs());
                string file = GetFileName();
                VersionInfo = SerializeHelper.Load(typeof(DiGitVersionInfo), file) as DiGitVersionInfo;
                SetData();
                UserManager.AddLog("Check Update", "Success");
            }
            catch (FileNotFoundException ex1)
            {
                LastReadError = new Exception("Version information not found.", ex1);
            }
            catch (Exception ex)
            {
                LastReadError = ex;
            }
            finally
            {
                if (LastReadError != null)
                {
                    ResetVars();
                    UserManager.AddLog("Check Update", "Error", LastReadError.Message);
                }
                LastReadDateTime = DateTime.Now;
                Working = false;
            }
            OnUpdateInfoChanged?.Invoke(null, new EventArgs());
        }

        /// <summary>
        /// Returns the file name by this priority:
        /// 1. Private file by user machine name (rename to ensure maintenance)
        /// 2. Beta user
        /// 3. Normal file
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            string file = GetFileName(Environment.UserName);
            if (File.Exists(file))
            {
                string fileDone = GetFileName(Environment.UserName + "_Done");
                File.Move(file, fileDone);
                return fileDone;
            }

            if (ConfigurationHelper.Configuration.isBetaUser)
            {
                file = GetFileName("beta");
                if (File.Exists(file)) return file;
            }


            return Path.Combine(Settings.Default.InfoUrl, string.Format(Settings.Default.InfoBaseFileName, ""));
        }

        private static string GetFileName(string subname)
        {
            if (subname != string.Empty) subname = "_" + subname;
            return Path.Combine(Settings.Default.InfoUrl, string.Format(Settings.Default.InfoBaseFileName, subname));
        }

        private static void ResetVars()
        {
            HasData = false;
            UpdateRequired = false;
            LastVersionInfo = null;
            SetupFileFound = false;
        }

        private static void SetData()
        {
            HasData = true;
            SetupFileFound = File.Exists(VersionInfo.Setup.URI);
            LastVersionInfo =
                VersionInfo.Version.FirstOrDefault(v => v.version.Equals(VersionInfo.Version.Max(v1 => v1.version)));
            if (LastVersionInfo != null)
                UpdateRequired = AppInfo.AppVersion.CompareTo(Version.Parse(LastVersionInfo.version)) < 0;
            else
                UpdateRequired = false;
            if (UpdateAvailable)
            {
                OnUpdateRequired?.Invoke(null, new EventArgs());
                ShowUpdateAvailableMessage();
            }

        }

        private static void ShowUpdateAvailableMessage()
        {
            NotificationHelper.ShowUpdateNotification(LastVersionInfo.version);
        }

        public static List<DiGitVersionInfoVersion> GetGreaterOrEqualVersions()
        {
            Version versionToCompare = Version.Parse(AppInfo.AppVersion.ToString(3));
            var versions =
                VersionInfo.Version.Where(v => Version.Parse(v.version).CompareTo(versionToCompare) > 0).ToList();

            if (!versions.Any())
            {
                // return a list with a single item - the same version
                versions = VersionInfo.Version.Where(v => Version.Parse(v.version).CompareTo(versionToCompare) == 0).ToList();
            }

            return versions;
        }

        public static void RunUpdate()
        {
            try
            {
                if (!File.Exists(VersionInfo.Setup.URI)) throw new FileNotFoundException();
                if (Msg.ShowQ("DiGit will be closed and restarted after setup is complete. Your settings will be saved.\nDo you want to continue?"))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = VersionInfo.Setup.URI;
                    p.Start();
                    new ExitCommand().Execute(null);
                }
            }
            catch (Exception ex)
            {
                Msg.ShowE(ex.Message);
            }
        }

        public static bool UpdateAvailable
        {
            get { return UpdateRequired && SetupFileFound; }
        }

        public static bool Working { get; private set; }

        public static bool HasData { get; private set; }

        public static bool UpdateRequired { get; private set; }

        public static bool SetupFileFound { get; private set; }

        public static DiGitVersionInfoVersion LastVersionInfo { get; private set; }

        public static Exception LastReadError
        {
            get { return _lastReadError; }
            private set
            {
                _lastReadError = value;
                if (value != null) ErrorHandler.Handle(value, false);
            }
        }

        public static DateTime LastReadDateTime { get; private set; }

    }
}
