namespace Omni
{
    using System;
    using System.Reactive;

    public interface IOmniService
    {
        #region Public Properties

        IObservable<OmniServiceStatusEnum> StatusChangedObservable { get; }

        OmniServiceStatusEnum State { get; }

        bool InTransition { get; }

        IObservable<bool> InTransitionObservable { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<Unit> Start();

        IObservable<Unit> Stop();

        #endregion
    }
}