namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface ISMSMessages
    {
        IObservable<EmptyModel> Send(string message, string phoneNumber);
    }
}