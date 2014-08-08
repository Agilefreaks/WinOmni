namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Notification.Models;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase<IncomingCallNotification>,
        IIncomingCallNotificationViewModel
    {
        #region Constructors and Destructors

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public IEventAggregator EventAggregator { get; set; }

        [Inject]
        public IPhones Phones { get; set; }

        #endregion

        #region Public Methods and Operators

        public void EndCall()
        {
            Phones.EndCall().Subscribe(p => TryClose(true), exception => { });
        }

        public void ReplyWithSms()
        {
            EventAggregator.PublishOnUIThread(new SendSmsMessage { Recipient = Model.PhoneNumber, Message = "" });
        }

        #endregion
    }
}