using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Core
{
    [TestClass]
    public class AnalyzerTests
    {
        private readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private readonly string BaseFolder = @"Base\Zoo";
        private readonly string WorkingFolder = nameof(Copy_Basics);

        private string SourcePath;
        private string WorkingPath;

        private IActivity _activity;
        private Analyzer _analyzer;

        [TestInitialize]
        public void Initialize()
        {
            WorkingPath = PathHelper.GetFullPath(BASE_PATH, WorkingFolder);
            if (PathHelper.Exists(WorkingPath))
                Cleanup();
            SourcePath = Path.Combine(WorkingPath, "Source");

            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, TalUtils.PathHelper.GetFullPath(SourcePath, true));

            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = SourcePath;

            _analyzer = new Analyzer(_activity);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete all created folders
            Directory.Delete(WorkingPath, true);
        }

        [TestMethod]
        public void ValidateFields_Success()
        {
            // arrange
            string expected = "";

            // act
            string actual = "error";
            try
            {
                _analyzer.ValidateFields();
            }
            catch (Exception ex)
            {
                actual = ex.Message;
            }

            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidateFields_NoDestinations_ThrowException()
        {
            // arrange

            // act

            // assert
            Assert.ThrowsException<NoDestinationsException>(() => _analyzer.ValidateFields());
        }

        [TestMethod]
        public void ValidateFields_NoSource_ThrowException()
        {
            // arrange
            _activity.Source.Path = "";

            // act

            // assert
            Assert.ThrowsException<NoSourceException>(() => _analyzer.ValidateFields());
        }

        [TestMethod]
        public void ValidateFields_DestinationEqualsSource_ThrowException()
        {
            // arrange
            _activity.Source.Path = @"c:\temp";
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = _activity.Source.Path
                }
                );

            // act

            // assert
            Assert.ThrowsException<DestinationEqualsSourceException>(() => _analyzer.ValidateFields());
        }

        [TestMethod]
        public void ValidateFields_SourcePathNotExists_ThrowException()
        {
            // arrange
            _activity.Source.Path = @"c:\dir not exist";

            // act

            // assert
            Assert.ThrowsException<SourceDirectoryNotFoundException>(() => _analyzer.ValidateFields());
        }

    }
}
