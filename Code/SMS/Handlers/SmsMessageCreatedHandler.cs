namespace SMS.Handlers
{
    using System;
    using OmniCommon.Handlers;
    using OmniCommon.Models;
    using SMS.Dto;
    using SMS.Resources.v1;

    public class SmsMessageCreatedHandler : ResourceHandler<SmsMessageDto>, ISmsMessageCreatedHandler
    {
        private readonly ISMSMessages _smsMessages;

        public override string HandledMessageType
        {
            get
            {
                return "sms_message_received";
            }
        }

        public SmsMessageCreatedHandler(ISMSMessages smsMessages)
        {
            _smsMessages = smsMessages;
        }

        protected override IObservable<SmsMessageDto> CreateResult(OmniMessage value)
        {
            var id = value.GetPayload("id");
            return _smsMessages.Get(id);
        }
    }
}