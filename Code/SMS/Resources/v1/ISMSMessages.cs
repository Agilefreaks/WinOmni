namespace SMS.Resources.v1
{
    using System;
    using System.Collections.Generic;
    using OmniCommon.Interfaces;
    using SMS.Dto;

    public interface ISMSMessages : IResource<SmsMessageDto>
    {
        IObservable<SmsMessageDto> Send(string phoneNumber, string message);

        IObservable<SmsMessageDto> Send(IList<string> phoneNumbers, string message);
    }
}