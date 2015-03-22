﻿namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Models;
    using PhoneCalls.Models;

    public interface IPhoneCallFactory
    {
        IObservable<T> Create<T>(PhoneCallDto phoneCallDto) where T : PhoneCall;
    }
}