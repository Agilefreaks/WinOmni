namespace SMS.Resources.v1
{
    using System;
    using Refit;
    using SMS.Dto;

    public interface ISMSMessagesApi
    {
        [Get("/sms_messages/{id}")]
        IObservable<SmsMessageDto> Get([AliasAs("id")] string id, [Header("Authorization")] string token);
        
        [Post("/sms_messages")]
        IObservable<SmsMessageDto> Create([Body] object payload, [Header("Authorization")] string token);
    }
}