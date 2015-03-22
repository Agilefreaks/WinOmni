﻿namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Models;
    using SMS.Models;

    public interface ISmsMessageFactory
    {
        IObservable<T> Create<T>(SmsMessageDto smsMessageDto) where T : SmsMessage;
    }
}