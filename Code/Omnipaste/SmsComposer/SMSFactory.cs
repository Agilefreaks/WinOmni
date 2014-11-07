namespace Omnipaste.SmsComposer
{
    using Omnipaste.EventAggregatorMessages;

    public class SMSFactory : ISMSFactory
    {
        //private static readonly string MessageSuffix = string.Empty;

        public SmsMessage Create()
        {
            return new SmsMessage { Message = string.Empty };
        }

        public SmsMessage Create(SendSmsMessage sendSmsMessage)
        {
            return new SmsMessage { Message = sendSmsMessage.Message, Recipient = sendSmsMessage.Recipient };
        }
    }
}