﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static bool LoadDefault()
        {
            _log.Info($"Loading default");
            if (File.Exists(DEFAULT_FILE))
                return Load(DEFAULT_FILE);
            return false;
        }



        public static void Create(PicPickProject project, string file)
        {
            Project = project;
            Save(file);
        }

        public static void Create(PicPickProject project)
        {
            Create(project, DEFAULT_FILE);
        }

        public static PicPickProject Project { get; set; }

        public static string FileName { get; private set; }

        public static bool Load(string file)
        {
            try
            {
                _log.Info($"Loading picpick file: {file}");
                Project = SerializeHelper.Load(typeof(PicPickProject), file) as PicPickProject;
                PrepareLoadedProject();
                FileName = file;
                return true;
            }
            catch (Exception ex)
            {
                _errorHandler.Handle(ex, true, "Error while loading configuration file '{0}'", file);
                return false;
            }
        }

        private static void PrepareLoadedProject()
        {
            foreach (var activity in Project.Activities)
            {
                foreach (var destination in activity.Destination)
                {
                    destination.Activity = activity;
                }
                
                // new filter objects in ver 2.0
                if (activity.Source.FromDate == null)
                {
                    activity.Source.FromDate = new DateComplex();
                    activity.Source.FromDate.Date = DateTime.Today.AddDays(-30);
                    activity.Source.ToDate = new DateComplex();
                    activity.Source.ToDate.Date = DateTime.Today;
                }
            }

        }

        public static string GetDefaultSchemaVersion()
        {
            try
            {
                return ((System.ComponentModel.DefaultValueAttribute)typeof(PicPickProject).GetProperty("ver").GetCustomAttribute(typeof(System.ComponentModel.DefaultValueAttribute))).Value.ToString();
            }
            catch (Exception ex)
            {
                _log.Error($"Could not get default schema version", ex);
                return AppInfo.AppVersionString;
            }
        }

        public static bool Save(string file)
        {

            try
            {
                _log.Info($"Saving picpick file: {file}");
                // get activities array
                Project.Activities = Project.ActivityList.Select(a => a as PicPickProjectActivity).ToArray();

                // get destination array in each task
                foreach (PicPickProjectActivity task in Project.Activities)
                    task.Destination = task.DestinationList.ToArray();

                Project.ver = GetDefaultSchemaVersion();

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
