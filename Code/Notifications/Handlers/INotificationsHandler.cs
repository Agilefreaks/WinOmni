namespace Notifications.Handlers
{
    using System;
    using Notifications.Models;

    public interface INotificationsHandler : IObservable<Notification>
    {
    }
}