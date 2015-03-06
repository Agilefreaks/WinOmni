namespace Omnipaste.Models
{
    using SMS.Models;

    public class LocalSmsMessage : SmsMessage
    {
        public LocalSmsMessage()
        {
        }

        public LocalSmsMessage(SmsMessageDto smsMessageDto)
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