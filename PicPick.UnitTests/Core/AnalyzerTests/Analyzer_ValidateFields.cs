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

namespace PicPick.UnitTests.Core.AnalyzerTests
{
    [TestClass]
    public class Analyzer_ValidateFields
    {
        
        private IActivity _activity;
        private Analyzer _analyzer;

        [TestInitialize]
        public void Initialize()
        {
            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = @"c:\";

            _analyzer = new Analyzer(_activity);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _analyzer = null;
            _activity = null;
        }

        [TestMethod]
        public void ValidateFields_Success()
        {
            // arrange
            string expected = "";
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = @"C:\test1"
                });

            // act
            string actual = "error";
            try
            {
                _analyzer.ValidateFields();
                actual = "";
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
            _activity.Source.Path = PathHelper.ExecutionPath();     // we must have an existing path
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination()
                {
                    Path = _activity.Source.Path,
                    Template = ""
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
