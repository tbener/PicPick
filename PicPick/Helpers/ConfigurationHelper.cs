using System;
using System.IO;
using PicPick.Configuration;
using log4net;
using TalUtils;

namespace PicPick.Helpers
{
    public static class ConfigurationHelper
    {
        private const string DEFAULT_FILE = "PicPick.xml";

        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private static PicPickConfig _PicPickConfig;

        public static PicPickConfig Default
        {
            get
            {
                if (_PicPickConfig == null)
                {
                    LoadedFile = PathHelper.GetFullPath(PathHelper.AppPath(@"Configuration"), DEFAULT_FILE);
                    Load(LoadedFile);
                }
                return _PicPickConfig;
            }
        }

        public static string LoadedFile { get; private set; }

        public static PicPickConfig Load(string file)
        {
            try
            {
                _PicPickConfig = SerializeHelper.Load(typeof(PicPickConfig), file) as PicPickConfig;
                //if (OnConfigurationLoaded != null)
                //    OnConfigurationLoaded(null, new EventArgs());
                return _PicPickConfig;
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
                foreach (PicPickConfigTask task in _PicPickConfig.Tasks)
                    task.Destination = task.DestinationList.ToArray();
                return SerializeHelper.Save(_PicPickConfig, file);
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
