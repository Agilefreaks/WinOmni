namespace Omni
{
    using System;
    using System.Reactive;
    using OmniCommon.Interfaces;
    using OmniSync;

    public interface IOmniService : IProxyConfigurationObserver
    {
        #region Public Properties

        IObservable<OmniServiceStatusEnum> StatusChangedObservable { get; }

        OmniServiceStatusEnum State { get; }

        bool InTransition { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<Unit> Start();

        IObservable<Unit> Stop();

        #endregion
    }
}