using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalUtils
{
    public class ExplorerHelper
    {
        public static void OpenFile(string fileName)
        {
            OpenFile(fileName, "open");
        }
        public static void OpenFile(string fileName, string command)
        {
            OpenFile(fileName, "open", "");
        }
        public static void OpenFile(string fileName, string command, string args)
        {
            Process p = new Process();
            p.StartInfo.Verb = command;
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = args;
            p.Start();
        }

        public static void OpenPath(string path)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Explorer.exe";
            p.StartInfo.Arguments = path;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();
        }

        public static void OpenContainingFolder(string fileName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "Explorer.exe";
            p.StartInfo.Arguments = "/Select," + fileName;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();
        }

        public static void BrowseUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url));
        }
    }
}
