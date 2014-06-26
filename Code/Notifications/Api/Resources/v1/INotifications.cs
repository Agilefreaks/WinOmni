namespace Notifications.Api.Resources.v1
{
    using System;

    public interface INotifications
    {
        IObservable<Models.Notification> Last();
    }
}