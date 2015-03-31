namespace Omnipaste.Notifications.NotificationList
{
    using System;
    using Omnipaste.Framework.Entities;
    using Omnipaste.Notifications.NotificationList.Notification;

    public interface INotificationViewModelFactory
    {
        IObservable<INotificationViewModel> Create(ClippingEntity clipping);

        IObservable<INotificationViewModel> Create(RemotePhoneCallEntity phoneCallEntity);

        IObservable<INotificationViewModel> Create(RemoteSmsMessageEntity smsMessageEntity);
    }
}