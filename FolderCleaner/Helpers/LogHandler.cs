using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderCleaner.Helpers
{
    public delegate void LogEventHandler(string msg);

    public static class LogHandler
    {
        
        public static event LogEventHandler OnLog;

        public static void Log(string msg)
        {
            string strDate = DateTime.Now.ToString("dd/MM/yy HH:mm");
            OnLog?.Invoke($"{strDate}: {msg}");
        }


        public static void Log(string file, string msg)
        {
            Log($"{file} - {msg}");
        }
    }
}
