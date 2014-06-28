namespace Notifications.Api.Resources.v1
{
    using System;
    using global::Notifications.Models;

    public interface INotifications
    {
        IObservable<Notification> Last();
    }
}