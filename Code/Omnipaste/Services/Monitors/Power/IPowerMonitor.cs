namespace Omnipaste.Services.Monitors.Power
{
    using System;
    using Microsoft.Win32;
    using Ninject;

    public interface IPowerMonitor : IStartable
    {
        IObservable<PowerModes> PowerModesObservable { get; }
    }
}