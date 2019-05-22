//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using PicPick.Helpers;
//using TalUtils;
//using System.Threading;
//using PicPick.Models;
//using System.Threading.Tasks;

//namespace PicPick.UnitTests
//{
//    /// <summary>
//    /// BASE_PATH 
//    ///     \ BaseFolder - Source files location, not to be deleted from.
//    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
//    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
//    /// 
//    /// </summary>
//    [TestClass]
//    public class Copy_Conflicts
//    {

//        private readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
//        private readonly string BaseFolder = @"Base\Zoo";
//        private readonly string WorkingFolder =  nameof(Copy_Basics);

//        private string SourcePath;
//        private string WorkingPath;

//        private PicPickProject _proj;
//        private PicPickProjectActivity _activity;
//        private List<string> _sourceFiles;

//        [TestInitialize]
//        public async Task Initialize()
//        {
//            // set pathes
//            WorkingPath = PathHelper.GetFullPath(BASE_PATH, WorkingFolder);
//            if (PathHelper.Exists(WorkingPath))
//                Cleanup();
//            SourcePath = Path.Combine(WorkingPath, "Source");

//            // initialize source files
//            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
//            ShellFileOperation.CopyItems(sourceFiles, TalUtils.PathHelper.GetFullPath(SourcePath, true));
//            _sourceFiles = new List<string>(Directory.GetFiles(SourcePath));

//            // initialize project
//            _proj = new PicPickProject();
//            _activity = new PicPickProjectActivity("test");
//            _activity.Source.Path = SourcePath;
//            _proj.ActivityList.Add(_activity);

//            // set destination
//            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
//            dest.Path = Path.Combine(WorkingPath, "Destination");
//            dest.Template = "";
//            _activity.DestinationList.Add(dest);

//            // execute to create the conflict (so the files will already be there for the test)
//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);
//        }

//        [TestCleanup]
//        public void Cleanup()
//        {
//            // delete all created folders
//            Directory.Delete(WorkingPath, true);
//        }

//        [TestMethod]
//        public async Task Copy_FilesExistsOvewrite_FileOverwriten()
//        {
//            _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.OVERWRITE;

//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

//            Assert.IsTrue(false);
//        }

//        [TestMethod]
//        public async Task Copy_FilesExistsSkip_FileSkipped()
//        {
//            // Arrange
//            _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.SKIP;
//            _activity.DeleteSourceFiles = true;
//            string checkFile = _sourceFiles.First();
//            Core.FILE_STATUS expectedStatus = Core.FILE_STATUS.SKIPPED;

//            // Act
//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

//            // Assert
//            // Verify the was not deleted
//            Assert.IsTrue(File.Exists(checkFile), "File deleted from source, even though it should have been skipped and left there.");
//            // Check file status
//            Core.FILE_STATUS actualStatus = _activity.FilesInfo[checkFile].Status;
//            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");
//        }

//        [TestMethod]
//        public async Task Copy_FilesExistsRename_FileRenamed()
//        {
//            _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.RENAME;

//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

//            Assert.IsTrue(false);
//        }

//        [TestMethod]
//        public async Task Copy_FilesExistsCompareSameFile_FileSkipped()
//        {
//            _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.COMPARE;

//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

//            Assert.IsTrue(false);
//        }

//        [TestMethod]
//        public async Task Copy_FilesExistsCompareDifferentFile_FileCopied()
//        {
//            _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.COMPARE;

//            await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

//            Assert.IsTrue(false);
//        }
//    }
//}
