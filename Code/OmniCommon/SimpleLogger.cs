namespace OmniCommon
{
    using System;
    using System.IO;
    using System.Threading;

    public class SimpleLogger
    {
        private static readonly object LockObject = new object();

        public static bool EnableLog { get; set; }

        public static void Log(string message)
        {
            if(!EnableLog) return;
            
            lock (LockObject)
            {
                using (var streamWriter = new StreamWriter(Path.Combine(Path.GetTempPath(), "Omnipaste_Log.txt"), true))
                {
                    streamWriter.WriteLine("{0} - {1} {2}", DateTime.Now, Thread.CurrentThread.ManagedThreadId, message);
                }
            }
        }
    }
}