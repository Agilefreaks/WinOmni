namespace Omnipaste.Entities
{
    using System;
    using PhoneCalls.Dto;

    public abstract class PhoneCallEntity : ConversationEntity
    {
        protected PhoneCallEntity()
        {
            Content = string.Empty;
        }

        protected PhoneCallEntity(PhoneCallDto phoneCallDto)
            : this()
        {
            Id = phoneCallDto.Id;
        }

        public abstract SourceType Source { get; }

        public string Content { get; set; }

        public PhoneCallEntity SetContactUniqueId(String contactUniqueId)
        {
            ContactUniqueId = contactUniqueId;
            return this;
        }
    }
}