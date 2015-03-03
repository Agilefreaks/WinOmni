namespace Omnipaste.Factories
{
    using System;
    using Omnipaste.Models;
    using SMS.Models;

    public interface ISmsMessageFactory<out T>
        where T : SmsMessage
    {
        IObservable<T> Create(SmsMessageDto smsMessage);
    }
}