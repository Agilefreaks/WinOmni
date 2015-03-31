namespace PhoneCalls.Handlers
{
    using System;
    using OmniCommon.Handlers;
    using OmniCommon.Models;
    using PhoneCalls.Dto;
    using PhoneCalls.Resources.v1;

    public class PhoneCallReceivedHandler : ResourceHandler<PhoneCallDto>, IPhoneCallReceivedHandler
    {
        private readonly IPhoneCalls _phoneCalls;

        public PhoneCallReceivedHandler(IPhoneCalls phoneCalls)
        {
            _phoneCalls = phoneCalls;
        }

        public override string HandledMessageType
        {
            get
            {
                return "phone_call_received";
            }
        }

        protected override IObservable<PhoneCallDto> CreateResult(OmniMessage value)
        {
            var id = value.GetPayload("id");
            return _phoneCalls.Get(id);
        }
    }
}