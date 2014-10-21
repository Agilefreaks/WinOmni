namespace Omnipaste
{
    using System;
    using System.Threading;

    public class SingleInstanceApp : IDisposable
    {
        #region Static Fields

        private static readonly TimeSpan DefaultAcquireMutexTimeout = TimeSpan.FromSeconds(2);

        #endregion

        #region Fields

        private readonly Mutex _mutex;

        #endregion

        #region Constructors and Destructors

        private SingleInstanceApp(string uniqueId, TimeSpan acquireMutexTimeout)
        {
            _mutex = new Mutex(false, uniqueId);
            // Wait a few seconds if contended, in case another instance
            // of the program is still in the process of shutting down.
            IsFirstInstance = _mutex.WaitOne(acquireMutexTimeout, false);
        }

        #endregion

        #region Public Properties

        public bool IsFirstInstance { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static bool InitializeAsFirstInstance(
            string uniqueId,
            out SingleInstanceApp instance,
            TimeSpan? acquireMutexTimeout = null)
        {
            acquireMutexTimeout = acquireMutexTimeout ?? DefaultAcquireMutexTimeout;
            instance = new SingleInstanceApp(uniqueId, acquireMutexTimeout.Value);

            return instance.IsFirstInstance;
        }

        public void Dispose()
        {
            if (_mutex != null)
            {
                _mutex.Close();
            }
        }

        #endregion
    }
}