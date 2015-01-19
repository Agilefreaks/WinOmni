namespace Omnipaste.Notification.IncomingSmsNotification
{
    using Caliburn.Micro;
    using Omnipaste.Properties;

    public class IncomingSmsNotificationViewModel : ConversationNotificationViewModelBase, IIncomingSmsNotificationViewModel
    {
        #region Constructors and Destructors

        public IncomingSmsNotificationViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #endregion

        #region Public Properties

        public override string Title
        {
            get
            {
                return Resources.IncommingSmsNotificationTitle;
            }
        }

        public override NotificationTypeEnum Type
        {
            get
            {
                return NotificationTypeEnum.IncomingSMS;
            }
        }

        #endregion
    }
}