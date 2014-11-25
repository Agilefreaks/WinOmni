namespace Omnipaste.EventAggregatorMessages
{
    public class SendSmsMessage
    {
        #region Public Properties

        public string Message { get; set; }

        public string Recipient { get; set; }

        #endregion
    }
}