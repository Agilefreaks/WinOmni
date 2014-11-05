namespace Omnipaste.Services.Monitors.User
{
    using System;

    public interface IUserMonitor
    {
        void SendEvent(UserEventTypeEnum eventType);

        IObservable<UserEventTypeEnum> UserEventObservable { get; }
    }
}