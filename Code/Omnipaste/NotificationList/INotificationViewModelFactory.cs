namespace Omnipaste.NotificationList
{
    using System;
    using Omnipaste.Entities;
    using Omnipaste.Models;
    using Omnipaste.NotificationList.Notification;

    public interface INotificationViewModelFactory
    {
        IObservable<INotificationViewModel> Create(ClippingEntity clipping);

        IObservable<INotificationViewModel> Create(RemotePhoneCallEntity phoneCallEntity);

        IObservable<INotificationViewModel> Create(RemoteSmsMessageEntity smsMessageEntity);
    }
}