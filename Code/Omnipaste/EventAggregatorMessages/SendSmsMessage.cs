namespace Omnipaste.EventAggregatorMessages
{
    public class SendSmsMessage
    {
        public string Recipient { get; set; }

        public string Message { get; set; }
    }
}