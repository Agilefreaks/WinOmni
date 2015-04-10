namespace Omnipaste.Framework.Models
{
    using Omnipaste.Framework.Entities;
    using OmniUI.Framework.Models;

    public abstract class SmsMessageModel : Model<SmsMessageEntity>, IConversationModel
    {
        protected SmsMessageModel(SmsMessageEntity backingEntity)
            : base(backingEntity)
        {
            BackingEntity = backingEntity;
        }

        #region IConversationModel Members

        public ContactModel ContactModel { get; set; }

        public SourceType Source
        {
            get
            {
                return BackingEntity.Source;
            }
        }

        public string Content
        {
            get
            {
                return BackingEntity.Content;
            }
        }

        public IConversationModel SetContactModel(ContactModel contactModel)
        {
            ContactModel = contactModel;
            return this;
        }

        #endregion
    }

    public abstract class SmsMessageModel<T> : SmsMessageModel, IModel<T>
        where T : SmsMessageEntity
    {
        protected SmsMessageModel(T smsMessage)
            : base(smsMessage)
        {
            BackingEntity = smsMessage;
        }

        #region IModel<T> Members

        public new T BackingEntity { get; set; }

        #endregion
    }
}