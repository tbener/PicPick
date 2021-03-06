﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Core;
using PicPick.Helpers;
using PicPick.Models;
using PicPick.Models.Interfaces;
using PicPick.Models.Mapping;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
    public class RunnerTestBaseClass
    {
        protected static string SUB_DIR = "";

        private static readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private static readonly string BaseFolder = @"Base\Zoo";

        protected static string SourcePath;
        protected static string DestinationPath;

        protected PicPickProject _project;
        protected PicPickProjectActivity _activity;

        public TestContext TestContext { get; set; }

        public async Task Run()
        {
            Reader reader = new Reader(_activity);
            Mapper mapper = new Mapper(_activity);
            Runner runner = new Runner(_activity, _project.Options);
            reader.ReadFiles();
            IProgressInformation progressInfo = new ProgressInformation();
            await mapper.MapAsync(progressInfo);
            mapper.ApplyFinalFilters();
            await runner.RunAsync(progressInfo);
        }

        public void InitActivity()
        {
            _activity = PicPickProjectActivity.CreateNew("test");
            _activity.FileGraph = new FilesGraph();
            _activity.Source.Path = SourcePath;

            _project = new PicPickProject();
            
        }

        public string GetWorkingFolder(string subDir)
        {
            return Path.Combine(TestContext.TestDir, subDir);
        }

        public static void InitFolders(TestContext testContext, string subDirectory)
        {
            string testFilesDir = Path.Combine(testContext.TestDir, subDirectory);
            SourcePath = PathHelper.GetFullPath(testFilesDir, "Source", true);
            DestinationPath = PathHelper.GetFullPath(testFilesDir, "Destination", true);

            CopyFilesTo(SourcePath);
        }

        public static void CopyFilesTo(string dir)
        {
            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, PathHelper.GetFullPath(dir, true));
        }

        protected PicPickProjectActivityDestination AddDestination(string path, string template = "")
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination(_activity);
            dest.Path = path;
            dest.Template = template;
            _activity.DestinationList.Add(dest);
            return dest;
        }

        protected void AssertStatus(FILE_STATUS expectedStatus, SourceFile checkedFile)
        {
            Assert.AreEqual(expectedStatus, checkedFile.Status, "File status was not set as expected.");
        }

        protected void AssertStatus(FILE_STATUS expectedStatus, string file)
        {
            SourceFile sourceFile = _activity.FileGraph.Files.Find(f => f.FullFileName.Equals(file, System.StringComparison.OrdinalIgnoreCase));
            if (sourceFile != null)
                AssertStatus(expectedStatus, sourceFile);
            else
                Assert.Fail($"File \"{file}\" is not found!");
        }

        /// <summary>
        /// As a file comparison we compare the size of the files.
        /// </summary>
        /// <param name="fileInfoExpected"></param>
        /// <param name="fileInfoActual"></param>
        protected void AssertFiles(FileInfo fileInfoExpected, FileInfo fileInfoActual)
        {
            Assert.AreEqual(fileInfoExpected.Length, fileInfoActual.Length, $"File size of {fileInfoActual.Name} in destination is not as expected.");
        }
    }
}
