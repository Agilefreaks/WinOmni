namespace Omni
{
    using System;
    using OmniApi.Models;
    using OmniSync;

    public interface IOmniService : IObservable<ServiceStatusEnum>
    {
        #region Public Events

        event EventHandler<ServiceStatusEventArgs> ConnectivityChanged;

        #endregion

        #region Public Properties

        ServiceStatusEnum Status { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<Device> Start();

        void Stop(bool unsubscribeHandlers = true);

        #endregion
    }
}