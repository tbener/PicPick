using System;
using System.IO;
using FolderCleaner.Configuration;
using log4net;
using TalUtils;

namespace FolderCleaner.Helpers
{
    public static class ConfigurationHelper
    {
        private const string DEFAULT_FILE = "FolderCleaner.xml";

        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private static FolderCleanerConfig _folderCleanerConfig;

        public static FolderCleanerConfig Default
        {
            get
            {
                if (_folderCleanerConfig == null)
                {
                    LoadedFile = PathHelper.GetFullPath(PathHelper.AppPath(@"Configuration"), DEFAULT_FILE);
                    Load(LoadedFile);
                }
                return _folderCleanerConfig;
            }
        }

        public static string LoadedFile { get; private set; }

        public static FolderCleanerConfig Load(string file)
        {
            try
            {
                _folderCleanerConfig = SerializeHelper.Load(typeof(FolderCleanerConfig), file) as FolderCleanerConfig;
                //if (OnConfigurationLoaded != null)
                //    OnConfigurationLoaded(null, new EventArgs());
                return _folderCleanerConfig;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, true, "Error while loading configuration file '{0}'", file);
                return null;
            }
        }

        public static bool Save(string file)
        {

            try
            {
                foreach (FolderCleanerConfigTask task in _folderCleanerConfig.Tasks)
                    task.Destination = task.DestinationList.ToArray();
                return SerializeHelper.Save(_folderCleanerConfig, file);
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, true, "Error while saving configuration file '{0}'", file);
                return false;
            }
        }
        public static bool Save()
        {
            return Save(LoadedFile);
        }
    }
}
