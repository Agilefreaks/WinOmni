namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public interface ISMSMessagesApi
    {
        [Post("/sms_messages")]
        IObservable<EmptyModel> Create([Body] object payload, [Header("Authorization")] string token);
    }
}