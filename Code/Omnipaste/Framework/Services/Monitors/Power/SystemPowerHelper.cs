namespace Omnipaste.Framework.Services.Monitors.Power
{
    using System;
    using System.Reactive.Subjects;
    using Microsoft.Win32;

    public class SystemPowerHelper : ISystemPowerHelper
    {
        private readonly Subject<PowerModeChangedEventArgs> _subject;

        public IObservable<PowerModeChangedEventArgs> PowerModeChangedObservable
        {
            get
            {
                return _subject;
            }
        }

        public SystemPowerHelper()
        {
            _subject = new Subject<PowerModeChangedEventArgs>();
        }

        private void SystemEventsOnPowerModeChanged(object sender, PowerModeChangedEventArgs powerModeChangedEventArgs)
        {
            _subject.OnNext(powerModeChangedEventArgs);
        }

        public void Start()
        {
            Stop();
            SystemEvents.PowerModeChanged += SystemEventsOnPowerModeChanged;
            // see http://msdn.microsoft.com/en-us/library/orm-9780596516109-03-19.aspx
            SystemEvents.EventsThreadShutdown += SystemEventsOnEventsThreadShutdown;
        }

        private void SystemEventsOnEventsThreadShutdown(object sender, EventArgs eventArgs)
        {
            Stop();
        }

        public void Stop()
        {
            SystemEvents.PowerModeChanged -= SystemEventsOnPowerModeChanged;
            SystemEvents.EventsThreadShutdown -= SystemEventsOnEventsThreadShutdown;
        }
    }
}