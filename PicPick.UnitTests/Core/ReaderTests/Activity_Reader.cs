﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Models;
using PicPick.Models.Interfaces;
using System.IO;
using TalUtils;

namespace PicPick.UnitTests.ReaderTests
{
    /// <summary>
    /// To Test:
    /// - Filter
    /// - IncludeSubFolders
    /// 
    /// For these tests we create dummy files just to test the reading by filter and subdirectories.
    /// The files will be deleted automatically because the TestDir is deleted by MSTest.
    /// </summary>
    [TestClass]
    public class Activity_Reader
    {
        const string SUB_DIR = @"_SourceFiles\FilterTest";
        private static string SourcePath;

        private static void CreateFiles(int count, string extension, string subfolder = "")
        {
            string dir = PathHelper.GetFullPath(SourcePath, subfolder, true) + "\\";
            for (int i = 1; i <= count; i++)
            {
                File.Create(dir + i.ToString("00") + extension);
            }
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            SourcePath = PathHelper.GetFullPath(testContext.TestDir, SUB_DIR, true);

            CreateFiles(10, ".jpg");
            CreateFiles(10, ".tmp");
            CreateFiles(5, ".jpg", "sub1");
            CreateFiles(5, ".tmp", "sub1");
            CreateFiles(10, ".tal");
        }


        [DataTestMethod]
        [DataRow("*.jpg", false, 10)]
        [DataRow("*.png", false, 0)]
        [DataRow("*.jpg", true, 15)]
        [DataRow("*.png", true, 0)]
        [DataRow("*.*", false, 30)]
        [DataRow("*.*", true, 40)]
        [DataRow("*.jpg,*.*", false, 30)]
        [DataRow("*.jpg  ;, *.*", false, 30)]
        [DataRow("jpg", false, 0)]
        [DataRow("", false, 30)]
        [DataRow("", true, 40)]
        [DataRow("*", true, 40)]
        [DataRow("blahblah*.*", false, 0)]
        [DataRow("blahblah,*.*", false, 30)]
        [DataRow("*.jpg, *.tal", false, 20)]
        [DataRow("*.jpg, *.tal", true, 25)]
        public void Reader_ReadFilesWithFilters_FileCount(string filter, bool includeSubFolders, int expectedCount)
        {
            // arrange
            IActivity _activity = PicPickProjectActivity.CreateNew(filter);
            _activity.FileGraph = new FilesGraph();
            _activity.Source.Path = SourcePath;
            _activity.Source.Filter = filter;
            _activity.Source.IncludeSubFolders = includeSubFolders;

            // act
            Reader reader = new Reader(_activity);
            reader.ReadFiles();

            // assert
            Assert.AreEqual(expectedCount, _activity.FileGraph.RawFileList.Count);
        }
    }
}
