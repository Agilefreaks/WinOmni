﻿namespace Omni
{
    using System;
    using System.Reactive;

    public interface IOmniService : IDisposable
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