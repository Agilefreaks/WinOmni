namespace SMS.Resources.v1
{
    using System;
    using Refit;
    using SMS.Models;

    public interface ISMSMessagesApi
    {
        [Get("/sms_messages/{id}")]
        IObservable<SmsMessage> Get([AliasAs("id")] string id, [Header("Authorization")] string token);
        
        [Post("/sms_messages")]
        IObservable<SmsMessage> Create([Body] object payload, [Header("Authorization")] string token);
    }
}