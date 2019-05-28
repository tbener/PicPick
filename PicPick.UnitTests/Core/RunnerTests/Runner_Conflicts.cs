using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [TestClass]
    public class Runner_Conflicts : RunnerTestBaseClass
    {
        private const string subDir = @"_SourceFiles\RunnerConflictsTest";

        private static List<string> _sourceFiles;

        private string _fileSource;
        private string _fileDestination;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitFolders(testContext, subDir);

            _sourceFiles = new List<string>(Directory.GetFiles(SourcePath));

            // create the conflict (so the files will already be there for the test)
            CopyFilesTo(DestinationPath);
        }

        [TestInitialize]
        public void TestInit()
        {
            InitActivity();
            
        }


        //[TestMethod]
        //public async Task Copy_FilesExistsOvewrite_FileOverwriten()
        //{
        //    _project.Options.FileExistsResponse = FileExistsResponseEnum.OVERWRITE;

        //    await Run();

        //    Assert.IsTrue(false);
        //}

        private void PrepareDifferentFilesWithSameName(string path)
        {
            File.Copy(_sourceFiles[0], Path.Combine(path, "source\\01.jpg"));
            File.Copy(_sourceFiles[1], Path.Combine(path, "destination\\01.jpg"));
        }

        [TestMethod]
        public async Task Conflict_SingleFileDifferentSkip_Skipped()
        {
            // Arrenge

            // 1. general settings
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;
            _activity.DeleteSourceFiles = true;

            // 2. files
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferentSkip_Skipped)));
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[1], Path.Combine(destPath, fileName));

            FileInfo fileInfoExpected = new FileInfo(_sourceFiles[1]);
            
            AddDestination(destPath);

            _activity.Source.Path = sourcePath;

            string checkedSourceFile = _activity.Source.FileList.First();

            // Act
            await Run();

            // Assert
            FileInfo fileInfoDest = new FileInfo(Path.Combine(destPath, fileName));

            // 1. File is not deleted from source
            Assert.IsTrue(File.Exists(checkedSourceFile), $"Source file disappeared: {checkedSourceFile}");

            // 2. file size in destination remained the same
            Assert.AreEqual(fileInfoExpected.Length, fileInfoDest.Length, "File size in destination is not as expected.");

            // 3. file status is Skipped
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

        }

        [TestMethod]
        [Ignore]
        public async Task Copy_FilesExistsSkip_FileSkipped()
        {
            // Arrange
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;
            _activity.DeleteSourceFiles = true;
            string checkFile = _sourceFiles.First();
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;
            AddDestination(DestinationPath);

            // Act
            await Run();

            // Assert
            // Verify the was not deleted
            Assert.IsTrue(File.Exists(checkFile), "File deleted from source, even though it should have been skipped and left there.");
            // Check file status
            FILE_STATUS actualStatus = _activity.FilesInfo[checkFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");
        }

        //[TestMethod]
        //public async Task Copy_FilesExistsRename_FileRenamed()
        //{
        //    _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.RENAME;

        //    await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

        //    Assert.IsTrue(false);
        //}

        //[TestMethod]
        //public async Task Copy_FilesExistsCompareSameFile_FileSkipped()
        //{
        //    _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.COMPARE;

        //    await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

        //    Assert.IsTrue(false);
        //}

        //[TestMethod]
        //public async Task Copy_FilesExistsCompareDifferentFile_FileCopied()
        //{
        //    _proj.Options.FileExistsResponse = Core.FileExistsResponseEnum.COMPARE;

        //    await _activity.Start(new ProgressInformation(), new CancellationTokenSource().Token);

        //    Assert.IsTrue(false);
        //}
    }
}
