namespace Omnipaste.Presenters
{
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using OmniUI.Presenters;

    public abstract class SmsMessagePresenter : Presenter<SmsMessageEntity>, IConversationPresenter
    {
        protected SmsMessagePresenter(SmsMessageEntity backingModel)
            : base(backingModel)
        {
            BackingModel = backingModel;
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

    public abstract class SmsMessagePresenter<T> : SmsMessagePresenter, IPresenter<T>
        where T : SmsMessageEntity
    {
        protected SmsMessagePresenter(T smsMessage)
            : base(smsMessage)
        {
            BackingModel = smsMessage;
        }

        #region IPresenter<T> Members

        public new T BackingModel { get; set; }

        #endregion
    }
}