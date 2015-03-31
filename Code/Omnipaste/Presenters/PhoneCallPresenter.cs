namespace Omnipaste.Presenters
{
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public abstract class PhoneCallPresenter : Presenter<PhoneCallEntity>, IConversationPresenter
    {
        protected PhoneCallPresenter(PhoneCallEntity phoneCallEntity)
            : base(phoneCallEntity)
        {
            BackingModel = phoneCallEntity;
        }

        #region IConversationPresenter Members

        public ContactInfoPresenter ContactInfoPresenter { get; set; }

        public SourceType Source
        {
            get
            {
                return BackingModel.Source;
            }
        }

        public string Content
        {
            get
            {
                return BackingModel.Content;
            }
        }

        public IConversationPresenter SetContactInfoPresenter(ContactInfoPresenter contactInfoPresenter)
        {
            ContactInfoPresenter = contactInfoPresenter;
            return this;
        }

        #endregion
    }

    public abstract class PhoneCallPresenter<T> : PhoneCallPresenter, IPresenter<T>
        where T : PhoneCallEntity
    {
        protected PhoneCallPresenter(T phoneCallEntity)
            : base(phoneCallEntity)
        {
            BackingModel = phoneCallEntity;
        }

        #region IPresenter<T> Members

        public new T BackingModel { get; set; }

        #endregion
    }
}