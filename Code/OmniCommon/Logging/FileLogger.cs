namespace OmniCommon.Logging
{
    using System;
    using System.IO;
    using System.Threading;
    using OmniCommon.Interfaces;

    public class FileLogger : ILogger
    {
        #region Constants

        private const string OutputFile = "Omnipaste_Log.txt";

        #endregion

        #region Static Fields

        private static readonly object LockObject = new object();

        private static FileLogger _instance;

        #endregion

        #region Constructors and Destructors

        private FileLogger()
        {
        }

        #endregion

        #region Public Properties

        public static FileLogger Instance
        {
            get
            {
                return _instance ?? (_instance = new FileLogger());
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Info(string message)
        {
            lock (LockObject)
            {
                using (var streamWriter = new StreamWriter(Path.Combine(Path.GetTempPath(), OutputFile), true))
                {
                    streamWriter.WriteLine("{0} - {1} {2}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message);
                }
            }
        }

        public void Error(Exception exception)
        {
            Info(exception.ToString());
        }

        #endregion
    }
}