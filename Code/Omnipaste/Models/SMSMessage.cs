namespace Omnipaste.Models
{
    using Omnipaste.Helpers;
    using SMS.Models;

    public abstract class SmsMessage : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        protected SmsMessage()
        {
            ContactInfo = new ContactInfo();
        }

        protected SmsMessage(SmsMessageDto smsMessageDto) : this()
        {
            Id = smsMessageDto.Id;
            Content = smsMessageDto.Content;
        }

        #endregion

        #region Public Properties

        public string Id { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public abstract SourceType Source { get; }

        #endregion
    }
}