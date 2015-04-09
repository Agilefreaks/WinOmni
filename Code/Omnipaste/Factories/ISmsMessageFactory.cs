namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Entities;
    using SMS.Dto;

    public interface ISmsMessageFactory
    {
        IObservable<T> Create<T>(SmsMessageDto smsMessageDto) where T : SmsMessageEntity;
    }
}