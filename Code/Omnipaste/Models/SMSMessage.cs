namespace Omnipaste.Models
{
    using System;
    using SMS.Models;

    public abstract class SmsMessage : ConversationBaseModel
    {
        protected SmsMessage()
        {
            Content = string.Empty;
        }

        protected SmsMessage(SmsMessageDto smsMessageDto)
            : this()
        {
            Id = smsMessageDto.Id;
            Content = smsMessageDto.Content;
        }

        public string Content { get; set; }

        public abstract SourceType Source { get; }

        public SmsMessage SetContactInfoUniqueId(String contactInfoUniqueId)
        {
            ContactInfoUniqueId = contactInfoUniqueId;
            return this;
        }
    }
}