namespace Omnipaste
{
    using System;
    using System.Threading;

    public class SingleInstanceApp : IDisposable
    {
        private readonly Mutex _mutex;

        public bool IsFirstInstance { get; private set; }

        private SingleInstanceApp(string uniqueId)
        {
            bool createdNew;
            _mutex = new Mutex(true, uniqueId, out createdNew);
            IsFirstInstance = createdNew;
        }

        public static bool InitializeAsFirstInstance(string uniqueId, out SingleInstanceApp instance)
        {
            instance = new SingleInstanceApp(uniqueId);

            return instance.IsFirstInstance;
        }

        public void Dispose()
        {
            if (_mutex != null)
            {
                _mutex.Close();
            }
        }
    }
}