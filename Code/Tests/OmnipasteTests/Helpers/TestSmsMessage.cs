namespace OmnipasteTests.Helpers
{
    using System;
    using Omnipaste.Models;
    using SMS.Models;

    public class TestSmsMessage : SmsMessage
    {
        public TestSmsMessage(SmsMessageDto smsMessageDto)
            : base(smsMessageDto)
        {
        }

        public TestSmsMessage()
            : base(new SmsMessageDto())
        {
        }

        public override SourceType Source
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}