using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PicPick.Helpers;
using TalUtils;
using PicPick.Project;
using System.Threading;
using PicPick.Models;
using System.Threading.Tasks;

namespace PicPick.UnitTests
{
    /// <summary>
    /// BASE_PATH 
    ///     \ BaseFolder - Source files location, not to be deleted from.
    ///     \ WorkingFolder - root folder of these tests. Should be deleted in the end. = WorkingPath
    ///         \ Source - the files are copied here as a start point for these tests. = SourcePath
    /// 
    /// </summary>
    [TestClass]
    public class Copy_Basics
    {

        private readonly string BASE_PATH = Path.GetFullPath(PathHelper.ExecutionPath(@"..\..\Test Files"));
        private readonly string BaseFolder = @"Base\Zoo";
        private readonly string WorkingFolder =  nameof(Copy_Basics);

        private string SourcePath;
        private string WorkingPath;

        private PicPickProject _proj;

        [TestInitialize]
        public void Initialize()
        {
            WorkingPath = PathHelper.GetFullPath(BASE_PATH, WorkingFolder);
            if (PathHelper.Exists(WorkingPath))
                Cleanup();
            SourcePath = Path.Combine(WorkingPath, "Source");

            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
            ShellFileOperation.CopyItems(sourceFiles, TalUtils.PathHelper.GetFullPath(SourcePath, true));

            _proj = new PicPickProject();
            PicPickProjectActivity act = new PicPickProjectActivity("test");
            act.Source.Path = SourcePath;
            _proj.ActivityList.Add(act);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // delete all created folders
            Directory.Delete(WorkingPath, true);
        }

        [TestMethod]
        public async Task Copy_NoTepmlate_FolderCreated()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
            string checkPath;
            dest.Path = Path.Combine(WorkingPath, "yyyy");
            dest.Template = "";
            act.DestinationList.Add(dest);

            Assert.IsFalse(dest.HasTemplate, "There was no Template supplied and the HasTemplate property returned true.");

            await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);

            // folder "yyyy"
            checkPath = Path.Combine(WorkingPath, "yyyy");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }

        [TestMethod]
        public async Task Copy_BasicTemlpate_FoldersCreated()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest;
            string checkPath;

            dest = new PicPickProjectActivityDestination();
            dest.Path = WorkingPath;
            dest.Template = "yyyy";
            act.DestinationList.Add(dest);

            dest = new PicPickProjectActivityDestination();
            dest.Path = WorkingPath;
            dest.Template = "yyyy-MM";
            act.DestinationList.Add(dest);

            Assert.IsTrue(dest.HasTemplate, "There is a Template supplied and the HasTemplate property returned false.");

            await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);

            
            // folder "2019"
            checkPath = Path.Combine(WorkingPath, "2019");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
            // folder "2019-05"
            checkPath = Path.Combine(WorkingPath, "2019-05");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }

        [TestMethod]
        public async Task Copy_TemlpateWithSubFolder_FoldersCreated()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest;
            string checkPath;

            dest = new PicPickProjectActivityDestination();
            dest.Path = WorkingPath;
            dest.Template = @"yyyy\\MM";
            act.DestinationList.Add(dest);

            await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


            // folder "2019\05"
            checkPath = Path.Combine(WorkingPath, "2019\\05");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }

        /// <summary>
        /// Destination has 2 properties that compose the full path - Path + Template.
        /// If the path is relative, it is relative to the Source, which means if both properties are empty,
        /// the destination is the source itself.
        /// The UI should have some sort of handling for that case, but there must be another protection from this situation in the engine level.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Copy_EmptyDestination_ThrowException()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
            dest.Path = "";
            dest.Template = "";
            act.DestinationList.Add(dest);


            await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await act.Start(new ProgressInformation(), new CancellationTokenSource().Token)
                );

            // Should raise error
        }

        [TestMethod]
        public async Task Copy_DestinationEqualsSource_ThrowException()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest = new PicPickProjectActivityDestination();
            dest.Path = SourcePath;
            dest.Template = "";
            act.DestinationList.Add(dest);


            await Assert.ThrowsExceptionAsync<Exception>(async () =>
                await act.Start(new ProgressInformation(), new CancellationTokenSource().Token)
                );

            // Should raise error
        }

        [TestMethod]
        public async Task Copy_DeleteSourceFilesFalse_SourceUnchanged()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest;
            act.DeleteSourceFiles = false;

            // get source start hash
            int hash1 = new DirectoryInfo(SourcePath).GetHashCode();

            dest = new PicPickProjectActivityDestination();
            dest.Path = WorkingPath;
            dest.Template = "yyyy";
            act.DestinationList.Add(dest);

            await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


            // compare hashes
            Assert.IsTrue(new DirectoryInfo(SourcePath).GetHashCode().Equals(hash1));
        }

        [TestMethod]
        public async Task Copy_DeleteSourceFilesTrue_SourceFilesDeleted()
        {
            PicPickProjectActivity act = _proj.ActivityList.First();
            PicPickProjectActivityDestination dest;
            act.DeleteSourceFiles = true;

            // get source content ?
            int hash1 = new DirectoryInfo(SourcePath).GetHashCode();

            dest = new PicPickProjectActivityDestination();
            dest.Path = WorkingPath;
            dest.Template = "yyyy";
            act.DestinationList.Add(dest);

            await act.Start(new ProgressInformation(), new CancellationTokenSource().Token);


            // verify source files deleted

            Assert.IsTrue(new DirectoryInfo(SourcePath).GetHashCode().Equals(hash1));
        }

        
    }
    
}
