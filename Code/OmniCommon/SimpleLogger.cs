namespace OmniCommon
{
    using System;
    using Microsoft.Practices.ServiceLocation;
    using OmniCommon.Interfaces;
    using OmniCommon.Logging;

    public static class SimpleLogger
    {
        #region Static Fields

        private static ILogger _logger;

        #endregion

        #region Public Properties

        public static bool EnableLog { get; set; }

        #endregion

        #region Properties

        private static ILogger Logger
        {
            get
            {
                return _logger
                       ?? (_logger =
                           ServiceLocator.IsLocationProviderSet
                               ? ServiceLocator.Current.GetInstance<ILogger>()
                               : new NullLoger());
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void Log(string message)
        {
            if (!EnableLog)
            {
                return;
            }

            Logger.Info(message);
        }
        
        public static void Log(string message, params object[] arguments)
        {
            Logger.Info(string.Format(message, arguments));
        }

        public static void Log(Exception exception)
        {
            if (!EnableLog)
            {
                return;
            }

            Logger.Error(exception);
        }

        #endregion
    }
}