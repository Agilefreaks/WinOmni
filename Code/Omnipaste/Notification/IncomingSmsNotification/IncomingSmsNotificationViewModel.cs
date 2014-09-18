namespace Omnipaste.Notification.IncomingSmsNotification
{
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Properties;

    public class IncomingSmsNotificationViewModel : NotificationViewModelBase, IIncomingSmsNotificationViewModel
    {
        #region Constructors and Destructors

        public IncomingSmsNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        public string PhoneNumber { get; set; }

        public override string Title
        {
            get
            {
                return string.Concat(Resources.IncommingSmsNotificationTitle, PhoneNumber);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Reply()
        {
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = PhoneNumber, Message = "" });
            Dismiss();
        }

        #endregion
    }
}