namespace Omnipaste.Framework.Services
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;
    using WindowsImports;
    using OmniCommon.Helpers;

    public class SystemIdleService : ISystemIdleService
    {
        #region Static Fields

        private static readonly TimeSpan InitialWaitTime = TimeSpan.Zero;

        private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(1);

        #endregion

        #region Public Methods and Operators

        public IObservable<bool> CreateSystemIdleObservable(TimeSpan idleThreshHold)
        {
            var timer = Observable.Timer(InitialWaitTime, RefreshInterval, SchedulerProvider.Default);

            return timer.Select(_ => GetIdleTime() > idleThreshHold);
        }

        #endregion

        #region Methods

        private static TimeSpan GetIdleTime()
        {
            return TimeSpan.FromMilliseconds(GetIdleTimeInMilliseconds());
        }

        private static long GetIdleTimeInMilliseconds()
        {
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.CbSize = Convert.ToUInt32(Marshal.SizeOf(lastInputInfo));
            if (!User32.GetLastInputInfo(ref lastInputInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return Environment.TickCount - lastInputInfo.DwTime;
        }

        #endregion
    }
}