namespace Omni
{
    using System;
    using System.Threading.Tasks;
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

        Task Start(string communicationChannel = null);

        void Stop(bool unsubscribeHandlers = true);

        #endregion
    }
}