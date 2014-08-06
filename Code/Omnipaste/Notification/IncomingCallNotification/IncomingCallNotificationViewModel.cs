namespace Omnipaste.Notification.IncomingCallNotification
{
    using System;
    using Ninject;
    using OmniApi.Resources.v1;
    using Omnipaste.Notification.Models;

    public class IncomingCallNotificationViewModel : NotificationViewModelBase<IncomingCallNotification>, IIncomingCallNotificationViewModel
    {
        #region Public Properties

        [Inject]
        public IPhones Phones { get; set; }

        #endregion

        #region Public Methods and Operators

        public void EndCall()
        {
            Phones.EndCall().Subscribe(
                p => { }, 
                exception => { });
        }

        #endregion
    }
}