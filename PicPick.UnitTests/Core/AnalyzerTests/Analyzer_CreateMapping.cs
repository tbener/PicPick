using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Core.AnalyzerTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    [TestClass]
    public class Analyzer_CreateMapping
    {
        const int TOTAL_FILE_COUNT = 4;

        private readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private readonly string BaseFolder = @"Base\Zoo";
        //private readonly string WorkingFolder = nameof(Copy_Basics);

        private string SourcePath;
        //private string WorkingPath;

        private IActivity _activity;
        private Analyzer _analyzer;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            SourcePath = PathHelper.GetFullPath(BASE_PATH, BaseFolder);

            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = SourcePath;
            _activity.Source.Filter = "";

            _analyzer = new Analyzer(_activity);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete all created folders
            //Directory.Delete(WorkingPath, true);
        }

        [TestMethod]
        public async Task CreateMapping_NoTemplate_OneDestinationFolder()
        {
            // arrange
            int expectedDestinationCount = 1;
            int expectedFileCount = TOTAL_FILE_COUNT;
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = "test",
                    Template = ""
                }
                );

            // act
            await _analyzer.CreateMapping(new ProgressInformation(), new CancellationTokenSource().Token);

            // assert
            Assert.AreEqual(expectedDestinationCount, _activity.Mapping.Count, "The amount of destination folders is not as expected.");
            foreach (var mappedFiles in _activity.Mapping.Values)
            {
                Assert.AreEqual(expectedFileCount, mappedFiles.FileList.Count, $"The amount of files for {mappedFiles.DestinationFolder} is not as expected.");
            }
        }

        [TestMethod]
        public async Task CreateMapping_WithTemplate_MultipleDestinationFolders()
        {
            // arrange
            int expectedFileCount = TOTAL_FILE_COUNT;
            var dest = new PicPickProjectActivityDestination()
            {
                Path = "test",
                Template = "MM-dd"
            };
            _activity.DestinationList.Add(dest);

            TestContext.WriteLine($"Template: {dest.Template} (DateTime.Now: {dest.GetTemplatePath(DateTime.Now)})");

            // act
            await _analyzer.CreateMapping(new ProgressInformation(), new CancellationTokenSource().Token);

            // assert
            Assert.IsTrue(_activity.Mapping.Count > 1, "The amount of destination folders is expected to be larger than 1.");
            int fileCountTotal = 0;
            foreach (var mappedFiles in _activity.Mapping.Values)
            {
                fileCountTotal += mappedFiles.FileList.Count;

                TestContext.WriteLine($"Destination: {mappedFiles.DestinationFolder}");
                foreach (var file in mappedFiles.FileList)
                {
                    TestContext.WriteLine($"\t {file}");
                }
            }
            Assert.AreEqual(expectedFileCount, fileCountTotal, $"The amount of files for all folders is different than the total amount of files.");
        }

    }
}
