using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Exceptions;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalUtils;

namespace PicPick.UnitTests.Models
{
    [TestClass]
    public class Activity_ValidateFields
    {

        private IActivity _activity;

        [TestInitialize]
        public void Initialize()
        {
            _activity = PicPickProjectActivity.CreateNew("test");
            _activity.Source.Path = @"c:\";

        }

        [TestCleanup]
        public void Cleanup()
        {
            _activity = null;
        }

        [TestMethod]
        public void ValidateFields_Success()
        {
            // arrange
            string expected = "";
            _activity.DestinationList.First().Path = @"C:\test1";

            // act
            string actual;
            try
            {
                _activity.ValidateFields();
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
            Assert.ThrowsException<NoDestinationsException>(() => _activity.ValidateFields());
        }

        [TestMethod]
        public void ValidateFields_NoSource_ThrowException()
        {
            // arrange
            _activity.Source.Path = "";

            // act

            // assert
            Assert.ThrowsException<NoSourceException>(() => _activity.ValidateFields());
        }

        /// <summary>
        /// Destination has 2 properties that compose the full path - Path + Template.
        /// If the path is relative, it is relative to the Source, which means if both properties are empty,
        /// the destination is the source itself.
        /// The UI should have some sort of handling for that case, but there must be another protection from this situation in the engine level.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public void ValidateFields_EmptyDestinationIsEqualsToSource_ThrowException()
        {
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination(_activity)
                {
                    Path = "",
                    Template = ""
                }
                );

            // act

            // assert
            // Should raise error
            Assert.ThrowsException<DestinationEqualsSourceException>(_activity.ValidateFields);

        }

        [TestMethod]
        public void ValidateFields_DestinationEqualsToSource_ThrowException()
        {
            // arrange
            _activity.Source.Path = PathHelper.ExecutionPath();     // we must have an existing path
            _activity.DestinationList.Add(
                new PicPickProjectActivityDestination(_activity)
                {
                    Path = _activity.Source.Path,
                    Template = ""
                }
                );

            // act

            // assert
            Assert.ThrowsException<DestinationEqualsSourceException>(() => _activity.ValidateFields());
        }

        [TestMethod]
        public void ValidateFields_SourceDirNotFound_ThrowException()
        {
            // arrange
            _activity.Source.Path = @"c:\dir not exist";

            // act

            // assert
            Assert.ThrowsException<SourceDirectoryNotFoundException>(() => _activity.ValidateFields());
        }

    }
}
