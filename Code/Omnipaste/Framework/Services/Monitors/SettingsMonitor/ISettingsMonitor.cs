namespace Omnipaste.Framework.Services.Monitors.SettingsMonitor
{
    using System;
    using Ninject;

    public interface ISettingsMonitor<out TSetting> : IStartable
    {
        IObservable<TSetting> SettingObservable { get; }
    }
}