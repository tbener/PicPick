using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;
using PicPick.Helpers;

namespace PicPick.UnitTests.Core.RunnerTests
{
    [TestClass]
    public class Runner_Conflicts_Ask : RunnerTestBaseClass
    {
        private const string subDir = @"_SourceFiles\RunnerConflictsAskTest";

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
            _project.Options.FileExistsResponse = FileExistsResponseEnum.ASK;

            EventAggregatorHelper.EventAggregator.GetEvent<FileExistsEvent>().Subscribe(OnFileExistsEvent);

        }

        private void OnFileExistsEvent(FileExistsEventArgs fileExistsEventArgs)
        {
            //fileExistsEventArgs.
        }

        [TestMethod]
        public async Task Conflict_SingleFileDifferent_Skip()
        {
            // Arrenge

            // 1. general settings
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileDifferent_Skip)));
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;

            // 2. files
            string sourcePath = PathHelper.GetFullPath(uniqueBasePath, "source", true);
            string destPath = PathHelper.GetFullPath(uniqueBasePath, "destination", true);
            string fileName = "01.jpg";
            File.Copy(_sourceFiles[0], Path.Combine(sourcePath, fileName));
            File.Copy(_sourceFiles[1], Path.Combine(destPath, fileName));
            
            AddDestination(destPath);

            _activity.Source.Path = sourcePath;

            string checkedSourceFile = _activity.Source.FileList.First();

            // Act
            await Run();

            // Assert
            FileInfo fileInfoDest = new FileInfo(Path.Combine(destPath, fileName));
            
            // 3. file status
            FILE_STATUS actualStatus = _activity.FilesInfo[checkedSourceFile].Status;
            Assert.AreEqual(expectedStatus, actualStatus, "File status was not set as expected.");

        }
    }
}
