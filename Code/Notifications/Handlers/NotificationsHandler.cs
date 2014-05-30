using System;
using System.Net;
using System.Reactive.Linq;
using Caliburn.Micro;
using Notifications.API;
using OmniCommon.Interfaces;
using OmniCommon.Models;

namespace Notifications.Handlers
{
    public class NotificationsHandler : IOmniMessageHandler
    {
        private readonly IEventAggregator _eventAggregator;

        private IDisposable _subscription;

        public INotificationsAPI NotificationsAPI { get; set; }

        public NotificationsHandler(INotificationsAPI notificationsAPI, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            NotificationsAPI = notificationsAPI;
        }

        public void OnNext(OmniMessage value)
        {
            var getAllNotificationsTask = NotificationsAPI.Last();
            getAllNotificationsTask.Wait();

            if (getAllNotificationsTask.Result.StatusCode == HttpStatusCode.OK)
            {
                _eventAggregator.PublishOnCurrentThread(getAllNotificationsTask.Result.Data);
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