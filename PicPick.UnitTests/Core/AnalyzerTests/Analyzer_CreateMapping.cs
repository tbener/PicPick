using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Exceptions;
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

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            SourcePath = PathHelper.GetFullPath(BASE_PATH, BaseFolder);

            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = SourcePath;
            _activity.Source.Filter = "";
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete all created folders
            //Directory.Delete(WorkingPath, true);
        }

        /// <summary>
        /// Destination has 2 properties that compose the full path - Path + Template.
        /// If the path is relative, it is relative to the Source, which means if both properties are empty,
        /// the destination is the source itself.
        /// The UI should have some sort of handling for that case, but there must be another protection from this situation in the engine level.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task CreateMapping_EmptyDestination_ThrowException()
        {
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = "",
                    Template = ""
                }
                );

            await Assert.ThrowsExceptionAsync<DestinationEqualsSourceException>(async () =>
                await _activity.FileMapping.Compute(new ProgressInformation())
                );

            // Should raise error
        }

        [TestMethod]
        public async Task CreateMapping_DestinationEqualsSource_ThrowException()
        {
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = SourcePath,
                    Template = ""
                }
                );

            await Assert.ThrowsExceptionAsync<DestinationEqualsSourceException>(async () =>
                await _activity.FileMapping.Compute(new ProgressInformation())
                );

            // Should raise error
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
            await _activity.FileMapping.Compute(new ProgressInformation());

            // assert
            Assert.AreEqual(expectedDestinationCount, _activity.FileMapping.DestinationFolders.Count, "The amount of destination folders is not as expected.");
            foreach (DestinationFolder destinationFolder in _activity.FileMapping.DestinationFolders.Values)
            {
                Assert.AreEqual(expectedFileCount, destinationFolder.Files.Count, $"The amount of files for {destinationFolder.FullPath} is not as expected.");
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
            await _activity.FileMapping.Compute(new ProgressInformation());

            var map = _activity.FileMapping;

            // assert
            Assert.IsTrue(map.DestinationFolders.Count > 1, "The amount of destination folders is expected to be larger than 1.");
            int fileCountTotal = 0;
            foreach (var destinationFolder in map.DestinationFolders.Values)
            {
                fileCountTotal += destinationFolder.Files.Count;

                TestContext.WriteLine($"Destination: {destinationFolder.FullPath}");
                foreach (var file in destinationFolder.Files)
                {
                    TestContext.WriteLine($"\t {file.SourceFile.FileName}");
                }
            }
            Assert.AreEqual(expectedFileCount, fileCountTotal, $"The amount of files for all folders is different than the total amount of files.");
        }

    }
}
