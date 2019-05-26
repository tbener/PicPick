using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PicPick.Helpers;
using TalUtils;
using PicPick.Models;
using System.Threading;
using System.Threading.Tasks;
using PicPick.Core;

namespace PicPick.UnitTests.Core.RunnerTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    /// 
    [TestClass]
    public class Runner_Run
    {
        const string SUB_DIR = @"_SourceFiles\RunnerTest";

        private static readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private static readonly string BaseFolder = @"Base\Zoo";

        private static string SourcePath;
        private static string DestinationPath;

        private PicPickProject _proj;
        private IActivity _activity;
        private Runner _runner;

        [ClassInitialize]
        public static void InitFiles(TestContext testContext)
        {
            SourcePath = PathHelper.GetFullPath(testContext.TestDir, SUB_DIR + "\\source", true);
            DestinationPath = PathHelper.GetFullPath(testContext.TestDir, SUB_DIR + "\\destination", true);

            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, PathHelper.GetFullPath(SourcePath, true));
        }

        [TestInitialize]
        public void InitActivity()
        {
            _activity = new PicPickProjectActivity("test");
            _activity.Source.Path = SourcePath;

            _proj = new PicPickProject();
            _runner = new Runner(_activity, _proj.Options);
        }


        [TestMethod]
        public async Task Runner_NoTemplate_FolderCreated()
        {
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
            dest.Path = Path.Combine(DestinationPath, "yyyy");
            dest.Template = "";
            _activity.DestinationList.Add(dest);

            Assert.IsFalse(dest.HasTemplate, "There was no Template supplied and the HasTemplate property returned true.");

            await _runner.Run(new ProgressInformation(), new CancellationTokenSource().Token);

            // folder "yyyy"
            string checkPath = Path.Combine(DestinationPath, "yyyy");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }
        

        //[TestMethod]
        //public async Task Copy_BasicTemlpate_FoldersCreated()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest;
        //    string checkPath;

        //    dest = new PicPickProjectActivityDestination();
        //    dest.Path = WorkingPath;
        //    dest.Template = "yyyy";
        //    act.DestinationList.Add(dest);

        //    dest = new PicPickProjectActivityDestination();
        //    dest.Path = WorkingPath;
        //    dest.Template = "yyyy-MM";
        //    act.DestinationList.Add(dest);

        //    Assert.IsTrue(dest.HasTemplate, "There is a Template supplied and the HasTemplate property returned false.");

        //    await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


        //    // folder "2019"
        //    checkPath = Path.Combine(WorkingPath, "2019");
        //    Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        //    // folder "2019-05"
        //    checkPath = Path.Combine(WorkingPath, "2019-05");
        //    Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        //}

        //[TestMethod]
        //public async Task Copy_TemlpateWithSubFolder_FoldersCreated()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest;
        //    string checkPath;

        //    dest = new PicPickProjectActivityDestination();
        //    dest.Path = WorkingPath;
        //    dest.Template = @"yyyy\\MM";
        //    act.DestinationList.Add(dest);

        //    await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


        //    // folder "2019\05"
        //    checkPath = Path.Combine(WorkingPath, "2019\\05");
        //    Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        //}

        ///// <summary>
        ///// Destination has 2 properties that compose the full path - Path + Template.
        ///// If the path is relative, it is relative to the Source, which means if both properties are empty,
        ///// the destination is the source itself.
        ///// The UI should have some sort of handling for that case, but there must be another protection from this situation in the engine level.
        ///// </summary>
        ///// <returns></returns>
        //[TestMethod]
        //public async Task Copy_EmptyDestination_ThrowException()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
        //    dest.Path = "";
        //    dest.Template = "";
        //    act.DestinationList.Add(dest);


        //    await Assert.ThrowsExceptionAsync<Exception>(async () =>
        //        await act.Start(new ProgressInformation(), new CancellationTokenSource().Token)
        //        );

        //    // Should raise error
        //}

        //[TestMethod]
        //public async Task Copy_DestinationEqualsSource_ThrowException()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
        //    dest.Path = SourcePath;
        //    dest.Template = "";
        //    act.DestinationList.Add(dest);


        //    await Assert.ThrowsExceptionAsync<Exception>(async () =>
        //        await act.Start(new ProgressInformation(), new CancellationTokenSource().Token)
        //        );

        //    // Should raise error
        //}

        //[TestMethod]
        //public async Task Copy_DeleteSourceFilesFalse_SourceUnchanged()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest;
        //    act.DeleteSourceFiles = false;

        //    // get source start hash
        //    DirectoryInfo di = new DirectoryInfo(SourcePath);
        //    string hash1 = GetDirectoryHash(di);

        //    dest = new PicPickProjectActivityDestination();
        //    dest.Path = WorkingPath;
        //    dest.Template = "yyyy";
        //    act.DestinationList.Add(dest);

        //    await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


        //    // compare hashes
        //    Assert.IsTrue(GetDirectoryHash(di).Equals(hash1));
        //}

        ///// <summary>
        ///// On this test we assume all files included (no specific filter) and no skipping.
        ///// So we expect an empty folder in the end.
        ///// </summary>
        ///// <returns></returns>
        //[TestMethod]
        //public async Task Copy_DeleteSourceFilesTrue_SourceFilesDeleted()
        //{
        //    PicPickProjectActivity act = _proj.ActivityList.First();
        //    PicPickProjectActivityDestination dest;
        //    act.DeleteSourceFiles = true;

        //    dest = new PicPickProjectActivityDestination();
        //    dest.Path = WorkingPath;
        //    dest.Template = "yyyy";
        //    act.DestinationList.Add(dest);

        //    await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


        //    // verify source files deleted
        //    Assert.IsTrue((new DirectoryInfo(SourcePath)).GetFiles().Count() == 0);
        //}

        //// Currently we just compare the first level file list
        //public string GetDirectoryHash(DirectoryInfo di)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    foreach (var file in di.GetFiles())
        //    {
        //        sb.Append(file.Name);
        //    }

        //    return sb.ToString();
        //}
    }
    
}
