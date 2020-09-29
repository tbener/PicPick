using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.Models.Mapping;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Core.MapperTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    [TestClass]
    public class Mapper_DestinationFolders
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

        private async Task RunMapping()
        {
            Reader reader = new Reader(_activity);
            Mapper mapper = new Mapper(_activity);
            reader.ReadFiles();
            await mapper.MapAsync(new ProgressInformation());
            mapper.ApplyFinalFilters();
        }
        
       
        [TestMethod]
        public async Task Map_NoTemplate_OneDestinationFolder()
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
            await RunMapping();

            // assert
            Assert.AreEqual(expectedDestinationCount, _activity.FilesGraph.DestinationFolders.Count, "The amount of destination folders is not as expected.");
            foreach (DestinationFolder destinationFolder in _activity.FilesGraph.DestinationFolders)
            {
                Assert.AreEqual(expectedFileCount, destinationFolder.Files.Count, $"The amount of files for {destinationFolder.FullPath} is not as expected.");
            }
        }

        [TestMethod]
        public async Task Map_WithTemplate_MultipleDestinationFolders()
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
            Reader reader = new Reader(_activity);
            Mapper mapper = new Mapper(_activity);
            reader.ReadFiles();
            await mapper.MapAsync(new ProgressInformation());
            mapper.ApplyFinalFilters();

            var filesGraph = _activity.FilesGraph;

            // assert
            Assert.IsTrue(filesGraph.DestinationFolders.Count > 1, "The amount of destination folders is expected to be larger than 1.");
            int fileCountTotal = 0;
            foreach (var destinationFolder in filesGraph.DestinationFolders)
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
