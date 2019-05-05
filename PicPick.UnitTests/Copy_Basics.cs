using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PicPick.Helpers;
using TalUtils;

namespace PicPick.UnitTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    [TestClass]
    public class Copy_Basics
    {

        private readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private readonly string BaseFolder = @"Base\Zoo";
        private readonly string WorkingFolder =  nameof(Copy_Basics);

        private string SourcePath;
        private string WorkingPath;

        [TestInitialize]
        public void InitFiles()
        {
            WorkingPath = PathHelper.GetFullPath(BASE_PATH, WorkingFolder);
            if (PathHelper.Exists(WorkingPath))
                Cleanup();
            SourcePath = Path.Combine(WorkingPath, "Source");

            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, TalUtils.PathHelper.GetFullPath(SourcePath, true));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete all created folders
            Directory.Delete(WorkingPath, true);
        }

        [TestMethod]
        public void Copy_()
        {

        }
    }
}
