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
using PicPick.Exceptions;

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
    public class Runner_Run : RunnerTestBaseClass
    {
        private const string subDir = @"_SourceFiles\RunnerBasicTest";

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            InitFolders(testContext, subDir);
        }

        [TestInitialize]
        public void TestInit()
        {
            InitActivity();
        }


        [TestMethod]
        public async Task Runner_NoTemplate_FolderCreated()
        {
            PicPickProjectActivityDestination dest = AddDestination(Path.Combine(DestinationPath, "yyyy"));

            Assert.AreEqual(1, _activity.DestinationList.Count());
            Assert.IsFalse(dest.HasTemplate, "There was no Template supplied and the HasTemplate property returned true.");

            await Run();

            // folder "yyyy"
            string checkPath = Path.Combine(DestinationPath, "yyyy");
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }

        [DataTestMethod]
        [DataRow("yyyy", "2019")]
        public async Task Runner_BasicTemlpate_FolderCreated(string template, string expectedFolder)
        {
            PicPickProjectActivityDestination dest = AddDestination(DestinationPath, template);

            Assert.AreEqual(1, _activity.DestinationList.Count());
            Assert.IsTrue(dest.HasTemplate, "There is a Template supplied and the HasTemplate property returned false.");

            await Run();

            // check that expected sub-folder exists
            string expectedPath = Path.Combine(DestinationPath, expectedFolder);
            Assert.IsTrue(Directory.Exists(expectedPath), $"Sub-folder {expectedFolder} doesn't exist.");
        }


        [DataTestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public async Task Runner_MultipleDestinations_FoldersCreated(string destFolder, string[] templates, string[] expectedFolders)
        {
            // Arrenge
            string destination = Path.Combine(DestinationPath, destFolder);

            foreach (string template in templates)
            {
                AddDestination(destination, template);
            }

            Assert.AreEqual(templates.Length, _activity.DestinationList.Count());

            // Act
            await Run();

            // Assert
            DirectoryInfo dirInfo = new DirectoryInfo(destination);
            var subDirs = new List<string>(dirInfo.GetDirectories().Select(d => d.Name));

            string checkPath;
            foreach (string dir in expectedFolders)
            {
                TestContext.WriteLine($"Checking {dir}...");
                checkPath = Path.Combine(destination, dir);
                Assert.IsTrue(Directory.Exists(checkPath), $"Folder {dir} doesn't exist.");

                Assert.IsTrue(subDirs.Contains(dir));
                subDirs.Remove(dir);
            }

            if (subDirs.Count() > 0)
            {
                string additionalDirs = "";
                TestContext.WriteLine("More folders created:");
                foreach (string dir in subDirs)
                {
                    additionalDirs += $"[{dir}] ";
                    TestContext.WriteLine($"\t{dir}");
                }
                Assert.Fail($"The folders {additionalDirs} were unexpectedly created.");
            }


        }

        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] { "1", new string[] { "yyyy", "yyyy-MM" }, new string[] { "2019", "2019-05" } };
            yield return new object[] { "2", new string[] { "yyyy", "yyyy-dd" }, new string[] { "2019", "2019-01", "2019-02", "2019-03" } };
        }

        [DataTestMethod]
        [DataRow(@"yyyy\\MM", "2019\\05")]
        public async Task Runner_TemlpateWithSubFolder_FoldersCreated(string template, string expected)
        {
            // Arrange
            AddDestination(DestinationPath, template);

            // Act
            await Run();

            // Assert
            string checkPath = Path.Combine(DestinationPath, expected);
            Assert.IsTrue(Directory.Exists(checkPath), $"Folder {checkPath} doesn't exist.");
        }

    }

}
