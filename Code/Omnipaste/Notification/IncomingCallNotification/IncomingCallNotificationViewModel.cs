namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase, IIncomingCallNotificationViewModel
    {
        #region Constructors and Destructors

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        public string PhoneNumber { get; set; }

        [Inject]
        public IDevices Devices { get; set; }

        public override string Title
        {
            get
            {
                return string.Concat("Incoming call from ", PhoneNumber);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void EndCall()
        {
            Devices.EndCall().Subscribe(p => TryClose(true), exception => { });
        }

        public void ReplyWithSms()
        {
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = PhoneNumber, Message = "" });
        }

        #endregion
    }
}