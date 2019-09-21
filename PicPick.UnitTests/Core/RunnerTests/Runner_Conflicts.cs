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
        

        [TestMethod]
        public async Task Conflict_SingleFileDifferent_Skip()
        {
            // Arrange

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferent_Skip)));
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;
            _activity.DeleteSourceFiles = true;

            // 2. files
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
            AssertFiles(fileInfoExpected, fileInfoDest);

            // 3. file status
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Conflict_SingleFileDifferent_Overwrite(bool deleteSourceFiles)
        {
            // Arrenge

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.OVERWRITE;
            string uniqueBasePath = GetWorkingFolder(
                Path.Combine(
                    subDir,
                    nameof(Conflict_SingleFileDifferent_Overwrite),
                    deleteSourceFiles ? "del_source" : "no_del_source"));
            FILE_STATUS expectedStatus = FILE_STATUS.COPIED;
            _activity.DeleteSourceFiles = deleteSourceFiles;

            // 2. files
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[1], Path.Combine(destPath, fileName));

            FileInfo fileInfoExpected = new FileInfo(_sourceFiles[0]);

            AddDestination(destPath);

            _activity.Source.Path = sourcePath;

            string checkedSourceFile = _activity.Source.FileList.First();

            // Act
            await Run();

            // Assert
            FileInfo fileInfoDest = new FileInfo(Path.Combine(destPath, fileName));

            // 1. File deletion from source
            string not = deleteSourceFiles ? string.Empty : "not ";
            Assert.AreEqual(!deleteSourceFiles, File.Exists(checkedSourceFile), $"Source file does {not}exist: {checkedSourceFile}");

            // 2. file size in destination remained the same
            AssertFiles(fileInfoExpected, fileInfoDest);

            // 3. file status
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

        }

        [TestMethod]
        public async Task Conflict_SingleFileDifferent_Rename()
        {
            // Arrenge

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.RENAME;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferent_Rename)));
            FILE_STATUS expectedStatus = FILE_STATUS.COPIED;
            _activity.DeleteSourceFiles = true;

            // 2. files
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[1], Path.Combine(destPath, fileName));

            FileInfo fileInfoExpected1 = new FileInfo(_sourceFiles[0]);
            FileInfo fileInfoExpected2 = new FileInfo(_sourceFiles[1]);

            AddDestination(destPath);

            _activity.Source.Path = sourcePath;

            string checkedSourceFile = _activity.Source.FileList.First();

            // Act
            await Run();

            // Assert

            // 1. verify both files exist
            var destFiles = Directory.GetFiles(destPath);

            Assert.AreEqual(2, destFiles.Count());
            foreach (var file in destFiles)
            {
                TestContext.WriteLine("File found: " + file);
                if (file.EndsWith(fileName))
                    AssertFiles(fileInfoExpected2, new FileInfo(file));
                else
                    AssertFiles(fileInfoExpected1, new FileInfo(file));
            }

            // 2. File is deleted from source
            Assert.IsFalse(File.Exists(checkedSourceFile), $"Source file supposed to be deleted: {checkedSourceFile}");

            // 3. file status 
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

        }

        [DataTestMethod]
        [DataRow("Same", "01.jpg", "01.jpg")]
        [DataRow("NotSame", "01.jpg", "02.jpg")]
        public async Task Conflict_SingleFileDifferent_Compare(string uid, string sourceFile1, string sourceFile2)
        {
            // Arrenge

            bool areEqual = sourceFile1.Equals(sourceFile2);

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.COMPARE;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferent_Compare), uid));
            FILE_STATUS expectedStatus = areEqual ? FILE_STATUS.SKIPPED : FILE_STATUS.COPIED;
            _activity.DeleteSourceFiles = true;
            int expectedCount = areEqual ? 1 : 2;
            bool expectedDeletedFromSource = !areEqual; // if files are same - skip (no deletion). if not same - copy and rename (delete from source dir)

            // 2. files
            string uniqueSourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string uniqueDestPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string file1 = Path.Combine(SourcePath, sourceFile1);
            string file2 = Path.Combine(SourcePath, sourceFile2);
            File.Copy(file1, Path.Combine(uniqueSourcePath, sourceFile1));
            File.Copy(file2, Path.Combine(uniqueDestPath, sourceFile1));

            FileInfo fileInfoExpected1 = new FileInfo(file1);
            FileInfo fileInfoExpected2 = new FileInfo(file2);

            AddDestination(uniqueDestPath);

            _activity.Source.Path = uniqueSourcePath;

            string checkedSourceFile = _activity.Source.FileList.First();

            // Act
            await Run();

            // 1. file status 
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

            // 2. verify file count in destination
            var destFiles = Directory.GetFiles(uniqueDestPath);

            Assert.AreEqual(expectedCount, destFiles.Count());
            foreach (var file in destFiles)
            {
                TestContext.WriteLine("File found: " + file);
                if (file.EndsWith(sourceFile1))
                    AssertFiles(fileInfoExpected2, new FileInfo(file));
                else
                    AssertFiles(fileInfoExpected1, new FileInfo(file));
            }

            // 3. File is deleted from source
            Assert.AreEqual(!expectedDeletedFromSource, File.Exists(checkedSourceFile), $"DeleteSourceFiles: expected {expectedDeletedFromSource}");

            

        }


        /// <summary>
        /// After an auto comapre is performed, some other action is performed.
        /// Need to test that on the next conflict we compare again and not performing the last action.
        /// We do that by having 2 conflicts that will result in different actions.
        /// </summary>
        [TestMethod]
        public async Task Conflict_FewFilesConflictCompare_PerformCompare()
        {
            // Arrenge

            List<string> fileNames = new List<string>() { "01.jpg", "02.jpg" };

            // 1. general settings
            _project.Options.FileExistsResponse = FileExistsResponseEnum.COMPARE;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferent_Compare)));

            // 2. files
            string uniqueSourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string uniqueDestPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            // copy to unique source
            fileNames.ForEach(f => File.Copy(Path.Combine(SourcePath, f), Path.Combine(uniqueSourcePath, f)));
            // copy one file as both files in dest
            string f1 = fileNames[0];
            fileNames.ForEach(f => File.Copy(Path.Combine(SourcePath, f1), Path.Combine(uniqueDestPath, f)));
            
            AddDestination(uniqueDestPath);

            _activity.Source.Path = uniqueSourcePath;

            // Act
            await Run();

            // 1. file status - expected first skip and second copy
            AssertStatus(FILE_STATUS.SKIPPED, Path.Combine(uniqueSourcePath, fileNames[0]));
            AssertStatus(FILE_STATUS.COPIED, Path.Combine(uniqueSourcePath, fileNames[1]));
            
        }

        
        
    }
}
