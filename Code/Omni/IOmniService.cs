namespace Omni
{
    using System;
    using System.Reactive;
    using OmniCommon.Models;

    public interface IOmniService : IDisposable
    {
        #region Public Properties

        IObservable<OmniServiceStatusEnum> StatusChangedObservable { get; }

        OmniServiceStatusEnum State { get; }

        bool InTransition { get; }

        IObservable<bool> InTransitionObservable { get; }

        IObservable<OmniMessage> OmniMessageObservable { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<Unit> Start();

        IObservable<Unit> Stop();

        #endregion
    }
}