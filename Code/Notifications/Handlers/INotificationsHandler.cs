namespace Notifications.Handlers
{
    using System;
    using Notifications.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public interface INotificationsHandler : IObservable<Notification>, IHandler, IObserver<OmniMessage>
    {
    }
}