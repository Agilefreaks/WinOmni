﻿namespace Omnipaste.Services
{
    using System;

    public interface ISystemIdleService
    {
        IObservable<bool> CreateSystemIdleObservable(TimeSpan idleThreshHold);
    }
}