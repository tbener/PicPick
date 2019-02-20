namespace PicPick.Helpers
{

    public static class LogHandler
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void Log(string msg, log4net.Core.Level level)
        {
            log.Logger.Log(log.GetType(), level, msg, null);
        }


        public static void Log(string file, string msg, log4net.Core.Level level)
        {
            Log($"{file} - {msg}", level);
        }

        public static void Log(string msg)
        {
            Log(msg, log4net.Core.Level.Info);
        }
    }
}
