namespace Omnipaste.Services.Monitors.Power
{
    using System;
    using Microsoft.Win32;
    using Ninject;

    public interface ISystemPowerHelper : IStartable
    {
        IObservable<PowerModeChangedEventArgs> PowerModeChangedObservable { get; }
    }
}