namespace SMS.Handlers
{
    using System;
    using OmniCommon.Handlers;
    using OmniCommon.Models;
    using SMS.Models;
    using SMS.Resources.v1;

    public class SmsMessageCreatedHandler : ResourceHandler<SmsMessage>, ISmsMessageCreatedHandler
    {
        private readonly ISMSMessages _smsMessages;

        public override string HandledMessageType
        {
            get
            {
                return "sms_message_created";
            }
        }

        public SmsMessageCreatedHandler(ISMSMessages smsMessages)
        {
            _smsMessages = smsMessages;
        }

        protected override IObservable<SmsMessage> CreateResult(OmniMessage value)
        {
            var id = value.GetPayload("id");
            return _smsMessages.Get(id);
        }
    }
}