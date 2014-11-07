namespace Omnipaste.SmsComposer
{
    using Omnipaste.EventAggregatorMessages;

    public interface ISMSFactory
    {
        SmsMessage Create();

        SmsMessage Create(SendSmsMessage sendSmsMessage);
    }
}