using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Models;
using System.IO;
using TalUtils;

namespace PicPick.UnitTests.Models
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
    public class Activity_Source
    {
        private static string SourcePath;

        
        private static void CreateFiles(int count, string extension, string subfolder = "")
        {
            string dir = PathHelper.GetFullPath(SourcePath, subfolder, true) + "\\";
            for (int i = 1; i <= count; i++)
            {
                File.Create(dir + i.ToString("00") + extension);
            }
        }

        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            
            SourcePath = PathHelper.GetFullPath(testContext.TestDir, "_SourceFiles", false);

            CreateFiles(10, ".jpg");
            CreateFiles(10, ".tmp");
            CreateFiles(5, ".jpg", "sub1");
            CreateFiles(5, ".tmp", "sub1");
            CreateFiles(10, ".tal");
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            //the files should be deleted by MSTest
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
        public void Source_Filter_FileList(string filter, bool includeSubFolders, int expectedCount)
        {
            // arrange
            var source = new PicPickProjectActivitySource()
            {
                Path = SourcePath,
                Filter = filter,
                IncludeSubFolders = includeSubFolders
            };

            // act
            var files = source.FileList;

            // assert
            Assert.AreEqual(expectedCount, files.Count);
        }
    }
}
