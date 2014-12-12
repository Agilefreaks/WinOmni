namespace Omnipaste.SmsComposer
{
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;

    public interface ISMSFactory
    {
        SMSMessage Create();

        SMSMessage Create(SendSmsMessage sendSmsMessage);
    }
}