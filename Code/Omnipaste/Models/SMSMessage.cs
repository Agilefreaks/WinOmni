namespace Omnipaste.Models
{
    using SMS.Models;

    public abstract class SmsMessage : BaseModel, IConversationItem
    {
        protected SmsMessage()
        {
            ContactInfo = new ContactInfo();
        }

        protected SmsMessage(SmsMessageDto smsMessageDto)
            : this()
        {
            Id = smsMessageDto.Id;
            Content = smsMessageDto.Content;
        }

        #region IConversationItem Members

        public string Id { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public abstract SourceType Source { get; }

        #endregion

        public SmsMessage SetContactInfo(ContactInfo contact)
        {
            ContactInfo = contact;
            return this;
        }
    }
}