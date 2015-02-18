namespace OmniDebug.Services
{
    using System;
    using SMS.Models;
    using SMS.Resources.v1;

    public class SmsMessagesWrapper : ResourceWrapperBase<SmsMessage>, ISmsMessagesWrapper
    {
        private readonly ISMSMessages _originalResource;

        public SmsMessagesWrapper(ISMSMessages originalResource)
            : base(originalResource)
        {
            _originalResource = originalResource;
        }

        public IObservable<SmsMessage> Send(string phoneNumber, string message)
        {
            return _originalResource.Send(phoneNumber, message);
        }
    }
}
