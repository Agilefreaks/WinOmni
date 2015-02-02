namespace Omnipaste.Notification
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;

    public abstract class ConversationNotificationViewModelBase : ResourceBasedNotificationViewModel<IConversationItem>,
                                                           IConversationNotificationViewModel
    {
        #region Fields

        private bool _canReplyWithSms = true;

        #endregion

        #region Constructors and Destructors

        protected ConversationNotificationViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public bool CanReplyWithSMS
        {
            get
            {
                return _canReplyWithSms;
            }
            set
            {
                if (value.Equals(_canReplyWithSms))
                {
                    return;
                }
                _canReplyWithSms = value;
                NotifyOfPropertyChange();
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        public override string Line1
        {
            get
            {
                return string.IsNullOrWhiteSpace(Resource.ContactInfo.Name) ? Resource.ContactInfo.Phone : Resource.ContactInfo.Name;
            }
        }

        public override string Line2
        {
            get
            {
                return Resource.Content;
            }
        }

        public override object Identifier
        {
            get
            {
                return Resource.UniqueId;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void ReplyWithSMS()
        {
            CanReplyWithSMS = false;
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = Resource.ContactInfo.Phone, Message = "" });
            Dismiss();
        }

        #endregion
    }
}