namespace SMS.Resources.v1
{
    using System;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using SMS.Models;

    public interface ISMSMessages : IResource<SmsMessage>
    {
        IObservable<EmptyModel> Send(string phoneNumber, string message);
    }
}