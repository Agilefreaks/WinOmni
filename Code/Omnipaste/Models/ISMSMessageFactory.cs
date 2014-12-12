namespace Omnipaste.Models
{
    using Omnipaste.EventAggregatorMessages;

    public interface ISMSMessageFactory
    {
        SMSMessage Create();

        SMSMessage Create(SendSmsMessage sendSmsMessage);
    }
}