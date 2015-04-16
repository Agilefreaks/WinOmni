namespace Omnipaste.Framework.Services.Monitors.Internet
{
    using System;

    public interface IConnectivityHelper
    {
        IObservable<bool> InternetConnectivityObservable { get; }
    }
}