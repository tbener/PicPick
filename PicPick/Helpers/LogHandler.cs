using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPick.Helpers
{
    // TEMP until I'll put in log4net
    public enum LOG_TYPE
    {
        INFO,
        ERROR,
        WARNING
    }

    public delegate void LogEventHandler(string msg, LOG_TYPE logType);

    public static class LogHandler
    {
        
        public static event LogEventHandler OnLog;

        public static void Log(string msg, LOG_TYPE logType)
        {
            //string strDate = DateTime.Now.ToString("dd/MM/yy HH:mm");
            OnLog?.Invoke(msg, logType);
        }


        public static void Log(string file, string msg, LOG_TYPE logType)
        {
            Log($"{file} - {msg}", logType);
        }

        public static void Log(string msg)
        {
            Log($"{msg}", LOG_TYPE.INFO);
        }
    }
}
