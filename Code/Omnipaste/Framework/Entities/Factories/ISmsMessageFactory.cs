namespace Omnipaste.Framework.Entities.Factories
{
    using System;
    using SMS.Dto;

    public interface ISmsMessageFactory
    {
        IObservable<T> Create<T>(SmsMessageDto smsMessageDto) where T : SmsMessageEntity;
    }
}