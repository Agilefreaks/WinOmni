namespace Omnipaste.Presenters
{
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public abstract class PhoneCallPresenter : Presenter<PhoneCall>, IConversationPresenter
    {
        protected PhoneCallPresenter(PhoneCall phoneCall)
            : base(phoneCall)
        {
            BackingModel = phoneCall;
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
        where T : PhoneCall
    {
        protected PhoneCallPresenter(T phoneCall)
            : base(phoneCall)
        {
            BackingModel = phoneCall;
        }

        #region IPresenter<T> Members

        public new T BackingModel { get; set; }

        #endregion
    }
}