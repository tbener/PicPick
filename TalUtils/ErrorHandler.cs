using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace TalUtils
{
    public class ErrorHandler
    {
        private ILog _logger;

        public ErrorHandler(ILog logger)
        {
            _logger = logger;
        }

        public ErrorHandler()
            : this(LogManager.GetLogger(typeof(ErrorHandler)))
        { }


        public static bool Handle(Exception error, string msg, bool displayMessage, ILog log)
        {
            bool retVal = true;

            Exception evtLogException = null;
            try
            {
                if (log != null)
                    log.Error(msg, error);
            }
            catch (Exception ex)
            {
                evtLogException = ex;
            }

            if (displayMessage)
                if (evtLogException == null)
                    retVal = Msg.ShowQ(error.Message);
                else
                    Msg.ShowE(error);

            if (evtLogException != null)
                Msg.ShowE(evtLogException, "An error occured while trying to write Log!");

            return retVal;
        }

        public static bool Handle(Exception error, string msg, bool displayMessage=true)
        {
            return Handle(error, msg, displayMessage);
        }


        public bool Handle(Exception error, bool displayMessage, string msg)
        {
            return Handle(error, msg, displayMessage, _logger);
        }

        public bool Handle(Exception error, bool displayMessage, string msg, params object[] args)
        {
            return Handle(error, displayMessage, string.Format(msg, args));
        }



        #region Public properties

        public ILog Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #endregion

        
    }
}
