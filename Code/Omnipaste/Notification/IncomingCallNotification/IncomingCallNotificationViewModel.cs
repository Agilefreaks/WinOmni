namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Caliburn.Micro;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Notification.Models;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase<IncomingCallNotification>, IIncomingCallNotificationViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        #region Public Properties

        [Inject]
        public IPhones Phones { get; set; }

        #endregion

        public IncomingCallNotificationViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #region Public Methods and Operators

        public void EndCall()
        {
            Phones.EndCall().Subscribe(
                p => TryClose(true), 
                exception => { });
        }

        #endregion

        public void ReplyWithSms()
        {
            //Show application with Send SMS view
            EventAggregator.PublishOnUIThread(new ShowShellMessage());

            Phones.SendSms(Model.PhoneNumber, "Can't talk right now").Subscribe(
                p => { },
                exception => { });
        }
    }
}