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
using PicPick.Models.Mapping;

namespace PicPick.UnitTests.Core.RunnerTests
{
    /// <summary>
    /// What we test here:
    /// In the raised event we return the desired action.
    /// 
    /// 1. Verify that the action was applied
    /// 2. Verify that next file on same batch is still asked (??)
    /// 3. 
    /// </summary>
    [TestClass]
    public class Runner_Conflicts_Ask : RunnerTestBaseClass
    {
        private const string subDir = @"_SourceFiles\RunnerConflictsAskTest";
        // Test Properties
        private const string EventRaisedCountProperty = "EventRaisedCount";
        private const string ReturnResponseFromEvent = "ReturnResponse";
        private const string DontAskAgainProperty = "DontAskAgain";

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

            TestContext.Properties.Add(EventRaisedCountProperty, 0);
            TestContext.Properties.Add(ReturnResponseFromEvent, FileExistsResponseEnum.SKIP);
            TestContext.Properties.Add(DontAskAgainProperty, false);

            EventAggregatorHelper.EventAggregator.GetEvent<FileExistsAskEvent>().Subscribe(OnFileExistsAskEvent);
        }

        [TestCleanup]
        public void TestClean()
        {
            EventAggregatorHelper.EventAggregator.GetEvent<FileExistsAskEvent>().Unsubscribe(OnFileExistsAskEvent);
        }

        private void OnFileExistsAskEvent(FileExistsAskEventArgs fileExistsAskEventArgs)
        {
            FileExistsResponseEnum returnResponse = FileExistsResponseEnum.SKIP;
            Enum.TryParse(TestContext.Properties[ReturnResponseFromEvent]?.ToString(), out returnResponse);

            int count = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            count++;

            TestContext.Properties[EventRaisedCountProperty] = count;
            fileExistsAskEventArgs.Response = returnResponse;
            fileExistsAskEventArgs.DontAskAgain = bool.Parse(TestContext.Properties[DontAskAgainProperty].ToString());
        }

        private void CreateConflicts(string basePath, int numberOfFiles = 1, string sourceFolder = "source", string destFolder = "destination")
        {
            string sourcePath = PathHelper.GetFullPath(basePath, sourceFolder, true);
            string destPath = PathHelper.GetFullPath(basePath, destFolder, true);

            string fileName;
            for (int i = 0; i < numberOfFiles; i++)
            {
                fileName = $"{i:00}.jpg";
                File.Copy(_sourceFiles[i], Path.Combine(sourcePath, fileName));
                File.Copy(_sourceFiles[i], Path.Combine(destPath, fileName));
            }

            _activity.Source.Path = sourcePath;
            AddDestination(destPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Conflict_SingleFileConflictAsk_RaiseEvent()
        {
            // Arrange

            // 1. general settings
            int expectedEventCount = 1;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileConflictAsk_RaiseEvent)));

            // 2. files
            CreateConflicts(uniqueBasePath, 1);

            // Act
            await Run();

            // Assert
            int actualEventCount = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            Assert.AreEqual(expectedEventCount, actualEventCount, "The 'Ask' Event was not raised as expected.");
        }

        [TestMethod]
        public async Task Conflict_SingleFileConflictDontAsk_EventNotRaised()
        {
            // Arrange

            // 1. general settings
            int expectedEventCount = 0;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileConflictDontAsk_EventNotRaised)));
            _project.Options.FileExistsResponse = FileExistsResponseEnum.SKIP;

            // 2. files - create an activity of one file
            CreateConflicts(uniqueBasePath, 1);

            // Act
            await Run();

            // Assert
            int actualEventCount = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            Assert.AreEqual(expectedEventCount, actualEventCount, "The 'Ask' Event was raised. Expected to Skip without raising event.");

        }

        /// <summary>
        /// The FileExistsResponse is set to ASK, but the Ask event is not handled (not subscribed).
        /// The expected resutls are:
        /// 1. No event method run (more of testing the test).
        /// 2. The default result is set - Skip.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Conflict_SingleFileConflictAsk_EventNotSubscribed()
        {
            // Arrange

            // 1. general settings
            EventAggregatorHelper.EventAggregator.GetEvent<FileExistsAskEvent>().Unsubscribe(OnFileExistsAskEvent);
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileConflictAsk_EventNotSubscribed)));
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;

            // 2. files
            CreateConflicts(uniqueBasePath, 1);

            // Act
            await Run();

            // Assert
            int actualEventCount = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            Assert.AreEqual(0, actualEventCount, "The 'Ask' Event was raised. Expected to Skip without raising event.");
            AssertStatus(expectedStatus, _activity.FileGraph.Files.First());

        }




        [TestMethod]
        public async Task Conflict_SingleFileConflictDontAskAgainFalse_AskEveryTime()
        {
            // Arrange

            // 1. general settings
            int expectedEventCount = 3;
            FILE_STATUS expectedStatus = FILE_STATUS.SKIPPED;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileConflictDontAskAgainFalse_AskEveryTime)));

            // 2. files
            CreateConflicts(uniqueBasePath, 3);

            // Act
            await Run();

            // Assert
            int actualEventCount = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            Assert.AreEqual(expectedEventCount, actualEventCount, "The 'Ask' Event was not raised as expected.");
            foreach (SourceFile file in _activity.FileGraph.Files)
            {
                AssertStatus(expectedStatus, file);
            }
        }

        [TestMethod]
        public async Task Conflict_SingleFileConflictDontAskAgainTrue_AskOnce()
        {
            // Arrange

            // 1. general settings
            int expectedEventCount = 1;
            TestContext.Properties[DontAskAgainProperty] = true;
            string uniqueBasePath = GetWorkingFolder(Path.Combine(subDir, nameof(Conflict_SingleFileConflictDontAskAgainTrue_AskOnce)));

            // 2. files
            CreateConflicts(uniqueBasePath + "-1", 3);
            CreateConflicts(uniqueBasePath + "-2", 3); // create a second set of mapping. had a bug on this.

            // Act
            await Run();

            // Assert
            int actualEventCount = int.Parse(TestContext.Properties[EventRaisedCountProperty].ToString());
            Assert.AreEqual(expectedEventCount, actualEventCount, "The 'Ask' Event was not raised as expected.");
        }
    }
}
