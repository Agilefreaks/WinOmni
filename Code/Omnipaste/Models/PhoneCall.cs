namespace Omnipaste.Models
{
    using PhoneCalls.Models;

    public abstract class PhoneCall : BaseModel, IConversationItem
    {
        protected PhoneCall()
        {
            ContactInfo = new ContactInfo();
            Content = string.Empty;
        }

        protected PhoneCall(PhoneCallDto phoneCallDto)
            : this()
        {
            Id = phoneCallDto.Id;
        }

        #region IConversationItem Members

        public string Id { get; set; }

        public ContactInfo ContactInfo { get; set; }

        public abstract SourceType Source { get; }

        public string Content { get; set; }

        #endregion

        public PhoneCall SetContactInfo(ContactInfo contact)
        {
            ContactInfo = contact;
            return this;
        }
    }
}