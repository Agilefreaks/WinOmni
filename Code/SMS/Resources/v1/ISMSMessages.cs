namespace SMS.Resources.v1
{
    using System;
    using OmniCommon.Interfaces;
    using SMS.Models;

    public interface ISMSMessages : IResource<SmsMessageDto>
    {
        IObservable<SmsMessageDto> Send(string phoneNumber, string message);
    }
}