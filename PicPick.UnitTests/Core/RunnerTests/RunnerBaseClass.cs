//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PicPick.UnitTests.Core.RunnerTests
//{
//    public class RunnerBaseClass
//    {
//        protected static string SUB_DIR = "";

//        public RunnerBaseClass(string subDir)
//        {
//            SUB_DIR = subDir;
//        }

//        [ClassInitialize]
//        public static void InitFiles(TestContext testContext)
//        {
//            SourcePath = PathHelper.GetFullPath(testContext.TestDir, SUB_DIR + "\\source", true);
//            DestinationPath = PathHelper.GetFullPath(testContext.TestDir, SUB_DIR + "\\destination", true);

//            var sourceFiles = new List<string>(Directory.GetFiles(Path.Combine(BASE_PATH, BaseFolder)));
//            ShellFileOperation.CopyItems(sourceFiles, PathHelper.GetFullPath(SourcePath, true));
//        }
//    }
//}
