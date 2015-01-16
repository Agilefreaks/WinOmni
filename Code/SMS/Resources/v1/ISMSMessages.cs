namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;
    using SMS.Models;

    public interface ISMSMessages
    {
        IObservable<EmptyModel> Send(string message, string phoneNumber);
        IObservable<SmsMessage> Get(string id);
        
    }
}