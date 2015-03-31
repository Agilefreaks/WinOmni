namespace Omnipaste.Models
{
    using Omnipaste.Entities;
    using OmniUI.Models;

    public abstract class PhoneCallModel : Model<PhoneCallEntity>, IConversationModel
    {
        protected PhoneCallModel(PhoneCallEntity backingEntity)
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

    public abstract class PhoneCallModel<T> : PhoneCallModel, IModel<T>
        where T : PhoneCallEntity
    {
        protected PhoneCallModel(T backingEntity)
            : base(backingEntity)
        {
            BackingEntity = backingEntity;
        }

        #region IModel<T> Members

        public new T BackingEntity { get; set; }

        #endregion
    }
}