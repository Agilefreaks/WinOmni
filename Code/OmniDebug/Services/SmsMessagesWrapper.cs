namespace OmniDebug.Services
{
    using System;
    using System.Collections.Generic;
    using SMS.Dto;
    using SMS.Resources.v1;

    public class SmsMessagesWrapper : ResourceWrapperBase<SmsMessageDto>, ISmsMessagesWrapper
    {
        private readonly ISMSMessages _originalResource;

        public SmsMessagesWrapper(ISMSMessages originalResource)
            : base(originalResource)
        {
            _originalResource = originalResource;
        }

        public IObservable<SmsMessageDto> Send(string phoneNumber, string message)
        {
            return _originalResource.Send(phoneNumber, message);
        }

        public IObservable<SmsMessageDto> Send(IList<string> phoneNumbers, string message)
        {
            return _originalResource.Send(phoneNumbers, message);
        }
    }
}
