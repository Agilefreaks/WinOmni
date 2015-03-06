namespace Omnipaste.Models
{
    using SMS.Models;

    public class RemoteSmsMessage : SmsMessage
    {
        public RemoteSmsMessage()
        {
        }

        public RemoteSmsMessage(SmsMessageDto smsMessageDto)
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