namespace Omnipaste.Services.Monitors.Power
{
    using System;
    using System.Reactive.Linq;
    using Microsoft.Win32;

    public class SystemPowerHelper : ISystemPowerHelper
    {
        public IObservable<PowerModeChangedEventArgs> PowerModeChangedObservable { get; private set; }

        public IObservable<EventArgs> EventsThreadShutdownObservable { get; private set; }

        public SystemPowerHelper()
        {
            PowerModeChangedObservable =
                Observable.FromEvent<PowerModeChangedEventHandler, PowerModeChangedEventArgs>(
                    handler => SystemEvents.PowerModeChanged += handler,
                    handler => SystemEvents.PowerModeChanged -= handler);
            EventsThreadShutdownObservable =
                Observable.FromEvent<EventHandler, EventArgs>(
                    handler => SystemEvents.EventsThreadShutdown += handler,
                    handler => SystemEvents.EventsThreadShutdown -= handler);
        }
    }
}