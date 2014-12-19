namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public interface ISMSMessagesApi
    {
        IObservable<EmptyModel> Create([Body]object payload, [Header("Authorization")] string token);
    }
}