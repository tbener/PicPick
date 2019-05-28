using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Core.RunnerTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    public class RunnerTestBaseClass
    {
        protected static string SUB_DIR = "";

        private static readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private static readonly string BaseFolder = @"Base\Zoo";

        protected static string SourcePath;
        protected static string DestinationPath;

        protected IActivity _activity;
        protected Runner _runner;

        public TestContext TestContext { get; set; }

        //public RunnerBaseClass(string subDir)
        //{
        //    SUB_DIR = subDir;
        //}

        public void InitActivity()
        {
            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = SourcePath;

            var _proj = new PicPickProject();
            _runner = new Runner(_activity, _proj.Options);
        }

        public static void InitFolders(TestContext testContext, string subDirectory)
        {
            SourcePath = PathHelper.GetFullPath(testContext.TestDir, subDirectory, true);
            DestinationPath = PathHelper.GetFullPath(testContext.TestDir, subDirectory, true);

            CopyFilesTo(SourcePath);
        }

        public static void CopyFilesTo(string dir)
        {
            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, PathHelper.GetFullPath(dir, true));
        }
    }
}
