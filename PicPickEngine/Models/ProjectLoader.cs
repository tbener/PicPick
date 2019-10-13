using System;
using System.IO;
using System.Linq;
using log4net;
using TalUtils;

namespace PicPick.Models
{
    public static class ProjectLoader
    {
        public static EventHandler OnSaveEventHandler;

        //  PathHelper.ExecutionPath() raises permissions issue after instalation.
        // private static readonly string DEFAULT_FILE = PathHelper.GetFullPath(PathHelper.ExecutionPath(), "Default.picpick");
        // Use My Documents instead:
        private static readonly string DEFAULT_FILE = PathHelper.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Default.picpick");

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ErrorHandler _errorHandler = new ErrorHandler(_log);


        public static bool IsDefaultFileName
        {
            get { return FileName.Equals(DEFAULT_FILE, StringComparison.CurrentCultureIgnoreCase); }
        }

        public static bool LoadCreateDefault()
        {
            if (File.Exists(DEFAULT_FILE))
            {
                if (Load(DEFAULT_FILE))
                    return true;
                if (!Msg.ShowQ("Project file could not be loaded. Do you want to create a new empty project?"))
                    return false;
            }
            Project = CreateNew("Default");
            return Save(DEFAULT_FILE);
        }

        public static PicPickProject CreateNew(string projectName)
        {
            PicPickProject proj = new PicPickProject()
            {
                Name = projectName
            };

            proj.ActivityList.Add(new PicPickProjectActivity("Default Activity"));

            return proj;
        }
        
        public static PicPickProject Project { get; set; }

        public static string FileName { get; private set; }

        public static bool Load(string file)
        {
            try
            {
                _log.Info($"Loading picpick file: {file}");
                Project = SerializeHelper.Load(typeof(PicPickProject), file) as PicPickProject;
                FileName = file;
                return true;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, true, "Error while loading configuration file '{0}'", file);
                return false;
            }
        }

        public static bool Save(string file)
        {

            try
            {
                // get activities array
                Project.Activities = Project.ActivityList.ToArray();

                // get destination array in each task
                foreach (PicPickProjectActivity task in Project.Activities)
                    task.Destination = task.DestinationList.ToArray();

                // save
                SerializeHelper.Save(Project, file);
                Project.IsDirty = false;
                FileName = file;
                OnSaveEventHandler?.Invoke(null, null);
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
            return Save(FileName);
        }
    }
}
