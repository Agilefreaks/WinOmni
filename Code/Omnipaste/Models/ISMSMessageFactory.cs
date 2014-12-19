namespace Omnipaste.Models
{
    using Omnipaste.EventAggregatorMessages;
    using OmniUI.Models;

    public interface ISMSMessageFactory
    {
        SMSMessage Create();

        SMSMessage Create(SendSmsMessage sendSmsMessage);

        SMSMessage Create(ContactInfo contactInfo);
    }
}