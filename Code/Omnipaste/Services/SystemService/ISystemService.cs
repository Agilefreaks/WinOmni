namespace Omnipaste.Services.SystemService
{
    using System;
    using Microsoft.Win32;
    using Ninject;

    public interface ISystemService : IStartable
    {
        IObservable<PowerModes> PowerModesObservable { get; }
    }
}