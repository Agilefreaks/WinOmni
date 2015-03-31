namespace Omnipaste.Entities
{
    using Omnipaste.Models;
    using SMS.Models;

    public class LocalSmsMessageEntity : SmsMessageEntity
    {
        public LocalSmsMessageEntity()
        {
        }

        public LocalSmsMessageEntity(SmsMessageDto smsMessageDto)
            : base(smsMessageDto)
        {
        }

        public override SourceType Source
        {
            get
            {
                return SourceType.Local;
            }
        }
    }
}