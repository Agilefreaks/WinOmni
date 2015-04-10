namespace OmnipasteTests.Framework.Helpers
{
    using System;
    using Omnipaste.Framework.Entities;
    using SMS.Dto;

    public class TestSmsMessageEntity : SmsMessageEntity
    {
        public TestSmsMessageEntity(SmsMessageDto smsMessageDto)
            : base(smsMessageDto)
        {
        }

        public TestSmsMessageEntity()
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