namespace Omnipaste.NotificationList
{
    using System;
    using Omnipaste.Models;
    using Omnipaste.Notification;

    public interface INotificationViewModelFactory
    {
        IObservable<INotificationViewModel> Create(ClippingModel clipping);

        IObservable<INotificationViewModel> Create(RemotePhoneCall phoneCall);

        IObservable<INotificationViewModel> Create(RemoteSmsMessage smsMessage);
    }
}