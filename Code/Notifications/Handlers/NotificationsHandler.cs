namespace Notifications.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Notifications.API;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class NotificationsHandler : IOmniMessageHandler
    {
        private readonly IEventAggregator _eventAggregator;

        private IDisposable _subscription;

        public INotificationsAPI NotificationsApi { get; set; }

        public NotificationsHandler(INotificationsAPI notificationsApi, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            this.NotificationsApi = notificationsApi;
        }

        public void OnNext(OmniMessage value)
        {
            var getAllNotificationsTask = this.NotificationsApi.Last();
            getAllNotificationsTask.Wait();

            if (getAllNotificationsTask.Result.StatusCode == HttpStatusCode.OK)
            {
                _eventAggregator.PublishOnUIThread(getAllNotificationsTask.Result.Data);
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(i => i.Provider == OmniMessageTypeEnum.Notification).Subscribe(this);
        }
    }
}