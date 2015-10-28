namespace Omnipaste.Framework
{
    using System;
    using NLog;
    using ILogger = OmniCommon.Interfaces.ILogger;

    public class NLogAdapter : ILogger
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static NLogAdapter _instance;

        #endregion

        #region Constructors and Destructors

        private NLogAdapter()
        {
        }

        #endregion

        #region Public Properties

        public static NLogAdapter Instance
        {
            get
            {
                return _instance ?? (_instance = new NLogAdapter());
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Error(Exception exception)
        {
            Logger.Error(exception);
        }

        #endregion
    }
}