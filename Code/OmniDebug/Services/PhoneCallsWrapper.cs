namespace OmniDebug.Services
{
    using System;
    using OmniApi.Models;
    using PhoneCalls.Models;
    using PhoneCalls.Resources.v1;

    public class PhoneCallsWrapper : ResourceWrapperBase<PhoneCallDto>, IPhoneCallsWrapper
    {
        private readonly IPhoneCalls _originalResource;

        public PhoneCallsWrapper(IPhoneCalls originalResource)
            : base(originalResource)
        {
            _originalResource = originalResource;
        }

        public IObservable<PhoneCallDto> Call(string phoneNumber, int? contactId = null)
        {
            return _originalResource.Call(phoneNumber);
        }

        public IObservable<EmptyModel> EndCall(string callId)
        {
            return _originalResource.EndCall(callId);
        }
    }
}