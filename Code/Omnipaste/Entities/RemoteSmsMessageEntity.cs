namespace Omnipaste.Models
{
    using Omnipaste.Entities;
    using SMS.Dto;

    public class RemoteSmsMessageEntity : SmsMessageEntity
    {
        public RemoteSmsMessageEntity()
        {
        }

        public RemoteSmsMessageEntity(SmsMessageDto smsMessageDto)
            : base(smsMessageDto)
        {
        }

        public override SourceType Source
        {
            get
            {
                return SourceType.Remote;
            }
        }
    }
}