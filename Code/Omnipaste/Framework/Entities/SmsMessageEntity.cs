namespace Omnipaste.Framework.Entities
{
    using System;
    using SMS.Dto;

    public abstract class SmsMessageEntity : ConversationEntity
    {
        protected SmsMessageEntity()
        {
            Content = string.Empty;
        }

        protected SmsMessageEntity(SmsMessageDto smsMessageDto)
            : this()
        {
            Id = smsMessageDto.Id;
            Content = smsMessageDto.Content;
        }

        public string Content { get; set; }

        public abstract SourceType Source { get; }

        public SmsMessageEntity SetContactUniqueId(String contactUniqueId)
        {
            ContactUniqueId = contactUniqueId;
            return this;
        }
    }
}