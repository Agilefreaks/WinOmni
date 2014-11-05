namespace Omni
{
    using System;
    using System.Reactive;
    using OmniCommon.Interfaces;
    using OmniSync;

    public interface IOmniService : IProxyConfigurationObserver
    {
        #region Public Properties

        IObservable<ServiceStatusEnum> StatusChangedObservable { get; }

        ServiceStatusEnum Status { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<Unit> Start();

        void StartWithDefaultObserver();

        IObservable<Unit> Stop();

        void StopWithDefaultObserver();

        #endregion
    }
}