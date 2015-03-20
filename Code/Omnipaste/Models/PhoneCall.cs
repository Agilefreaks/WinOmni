namespace Omnipaste.Models
{
    using System;
    using PhoneCalls.Models;

    public abstract class PhoneCall : ConversationBaseModel
    {
        protected PhoneCall()
        {
            Content = string.Empty;
        }

        protected PhoneCall(PhoneCallDto phoneCallDto)
            : this()
        {
            Id = phoneCallDto.Id;
        }

        public abstract SourceType Source { get; }

        public string Content { get; set; }

        public PhoneCall SetContactInfoUniqueId(String contactInfoUniqueId)
        {
            ContactInfoUniqueId = contactInfoUniqueId;
            return this;
        }
    }
}