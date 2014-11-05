namespace Omnipaste.Services.SystemService
{
    using System;
    using Microsoft.Win32;

    public interface ISystemPowerHelper
    {
        IObservable<EventArgs> EventsThreadShutdownObservable { get; }

        IObservable<PowerModeChangedEventArgs> PowerModeChangedObservable { get; }
    }
}