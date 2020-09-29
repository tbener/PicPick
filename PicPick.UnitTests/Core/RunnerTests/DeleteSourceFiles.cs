using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Models.Mapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Core.RunnerTests
{
    [TestClass]
    public class DeleteSourceFiles : RunnerTestBaseClass
    {
        private const string subDir = @"_SourceFiles\DeleteSourceFilesTest";
        private static List<string> _sourceFiles;

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

        // Currently we just compare the first level file list
        private string GetDirectoryHash(DirectoryInfo di)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var file in di.GetFiles())
            {
                sb.Append(file.Name);
            }

            return sb.ToString();
        }


        private string GetUniquePath(string uniqueString)
        {
            return GetWorkingFolder(Path.Combine(subDir, uniqueString));
        }

        [TestMethod]
        public async Task DeleteSourceFiles_DeleteFalse_SourceNotDeleted()
        {
            _activity.DeleteSourceFiles = false;
            _activity.DeleteSourceFilesOnSkip = false;

            // get source start hash
            DirectoryInfo di = new DirectoryInfo(SourcePath);
            string hash1 = GetDirectoryHash(di);

            AddDestination(DestinationPath, "yyyy");

            await Run();

            // compare hashes
            Assert.IsTrue(GetDirectoryHash(di).Equals(hash1));
        }

        [TestMethod]
        public async Task DeleteSourceFiles_DeleteFalseOnSkipTrue_SourceNotDeleted()
        {
            _activity.DeleteSourceFiles = false;
            _activity.DeleteSourceFilesOnSkip = true;

            // get source start hash
            DirectoryInfo di = new DirectoryInfo(SourcePath);
            string hash1 = GetDirectoryHash(di);

            AddDestination(DestinationPath, "yyyy");

            await Run();

            // compare hashes
            Assert.IsTrue(GetDirectoryHash(di).Equals(hash1));
        }

        /// <summary>
        /// On this test we assume all files included (no specific filter) and no skipping.
        /// So we expect an empty folder in the end.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DeleteSourceFiles_DeleteTrueOnSkipFalse_SourceFilesDeleted()
        {
            // Arrange
            _activity.DeleteSourceFiles = true;
            _activity.DeleteSourceFilesOnSkip = false;

            // Copy files to a new source folder - those files are expected to be deleted
            var newSourcePath = GetUniquePath(nameof(DeleteSourceFiles_DeleteTrueOnSkipFalse_SourceFilesDeleted));
            CopyFilesTo(newSourcePath);

            // set the new source
            _activity.Source.Path = newSourcePath;
            _activity.Source.Filter = "*.*";

            // set the destination
            AddDestination(Path.Combine(DestinationPath, nameof(DeleteSourceFiles_DeleteTrueOnSkipFalse_SourceFilesDeleted)));

            // Act
            await Run();

            // verify source files deleted
            Assert.AreEqual(0, (new DirectoryInfo(newSourcePath)).GetFiles().Count());
        }

        [TestMethod]
        public async Task DeleteSourceFiles_DeleteTrueOnSkipTrue_SourceFilesDeleted()
        {
            // Arrange
            _activity.DeleteSourceFiles = true;
            _activity.DeleteSourceFilesOnSkip = true;

            // Copy files to a new source folder - those files are expected to be deleted
            var newSourcePath = GetUniquePath(nameof(DeleteSourceFiles_DeleteTrueOnSkipTrue_SourceFilesDeleted));
            CopyFilesTo(newSourcePath);

            // set the new source
            _activity.Source.Path = newSourcePath;
            _activity.Source.Filter = "*.*";

            // set the destination
            AddDestination(Path.Combine(DestinationPath, nameof(DeleteSourceFiles_DeleteTrueOnSkipTrue_SourceFilesDeleted)));

            // Act
            await Run();

            // verify source files deleted
            Assert.AreEqual(0, (new DirectoryInfo(newSourcePath)).GetFiles().Count());
        }


        [TestMethod]
        public async Task DeleteSourceFiles_DeleteTrueOnSkipFalse_SkippedFilesNotDeleted()
        {
            // Arrange
            _activity.DeleteSourceFiles = true;
            _activity.DeleteSourceFilesOnSkip = false;

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;
            string uniqueBasePath = GetUniquePath(nameof(DeleteSourceFiles_DeleteTrueOnSkipFalse_SkippedFilesNotDeleted));

            // 2. files
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[0], Path.Combine(destPath, fileName));

            AddDestination(destPath);
            _activity.Source.Path = sourcePath;

            // Act
            await Run();

            // Assert

            // File is skipped, and not deleted from source
            SourceFile checkSourceFile = _activity.FilesGraph.Files.First();
            AssertStatus(FILE_STATUS.SKIPPED, checkSourceFile);
            Assert.IsTrue(File.Exists(checkSourceFile.FullFileName), $"Source file disappeared: {checkSourceFile.FileName}");
        }

        [TestMethod]
        public async Task DeleteSourceFiles_DeleteTrueOnSkipTrue_SkippedFilesDeleted()
        {
            // Arrange
            _activity.DeleteSourceFiles = true;
            _activity.DeleteSourceFilesOnSkip = true;

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;
            string uniqueBasePath = GetUniquePath(nameof(DeleteSourceFiles_DeleteTrueOnSkipTrue_SkippedFilesDeleted));

            // 2. files
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[0], Path.Combine(destPath, fileName));

            AddDestination(destPath);
            _activity.Source.Path = sourcePath;

            // Act
            await Run();

            // Assert

            // File is skipped, and not deleted from source
            SourceFile checkSourceFile = _activity.FilesGraph.Files.First();
            AssertStatus(FILE_STATUS.SKIPPED, checkSourceFile);
            Assert.IsFalse(File.Exists(checkSourceFile.FullFileName), $"Source file {checkSourceFile.FileName} was skipped, but not deleted");
        }


    }
}
