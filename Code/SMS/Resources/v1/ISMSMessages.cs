namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;
    using SMS.Models;

    public interface ISMSMessages
    {
        IObservable<EmptyModel> Send(string phoneNumber, string message);

        IObservable<SmsMessage> Get(string id);
    }
}