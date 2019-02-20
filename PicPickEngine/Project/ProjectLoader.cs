using System;
using System.Linq;
using log4net;
using TalUtils;

namespace PicPick.Project
{
    public static class ConfigurationHelper
    {
        private const string DEFAULT_FILE = "PicPick.xml";

        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);

        private static PicPickProject _picPickProject;

        public static PicPickProject Default
        {
            get
            {
                if (_picPickProject == null)
                {
                    LoadedFile = PathHelper.GetFullPath(PathHelper.AppPath(@"Configuration"), DEFAULT_FILE);
                    Load(LoadedFile);
                }
                return _picPickProject;
            }
        }

        public static string LoadedFile { get; private set; }

        public static PicPickProject Load(string file)
        {
            try
            {
                _picPickProject = SerializeHelper.Load(typeof(PicPickProject), file) as PicPickProject;
                //if (OnConfigurationLoaded != null)
                //    OnConfigurationLoaded(null, new EventArgs());
                return _picPickProject;
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
                // get tasks array
                _picPickProject.Activities = _picPickProject.ActivityList.ToArray();

                // get destination array in each task
                foreach (PicPickProjectActivity task in _picPickProject.Activities)
                    task.Destination = task.DestinationList.ToArray();

                // save
                SerializeHelper.Save(_picPickProject, file);
                _picPickProject.IsDirty = false;
                return true;
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
