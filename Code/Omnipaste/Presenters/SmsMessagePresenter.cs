namespace Omnipaste.Presenters
{
    using Omnipaste.Models;

    public abstract class SmsMessagePresenter : Presenter<SmsMessage>, IConversationPresenter
    {
        protected SmsMessagePresenter(SmsMessage backingModel)
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
        where T : SmsMessage
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