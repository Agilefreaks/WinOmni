namespace Omnipaste.Services
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Runtime.InteropServices;
    using WindowsImports;

    public class SystemIdleService : ISystemIdleService
    {
        private static readonly TimeSpan InitialWaitTime = TimeSpan.Zero;
        private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(10);

        public IObservable<bool> CreateSystemIdleObservable(TimeSpan idleThreshHold)
        {
            var timer = Observable.Timer(InitialWaitTime, RefreshInterval);

            return timer.Select(_ => GetIdleTime() > idleThreshHold);
        }

        private static TimeSpan GetIdleTime()
        {
            return TimeSpan.FromTicks(GetIdleTimeInTicks());
        }

        private static long GetIdleTimeInTicks()
        {
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.CbSize = Convert.ToUInt32((Marshal.SizeOf(lastInputInfo)));
            if (!User32.GetLastInputInfo(ref lastInputInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return Convert.ToUInt32(Environment.TickCount) - lastInputInfo.DwTime;
        }
    }
}