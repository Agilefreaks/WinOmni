namespace OmnipasteTests.Helpers
{
    using System;
    using Omnipaste.Entities;
    using Omnipaste.Models;
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