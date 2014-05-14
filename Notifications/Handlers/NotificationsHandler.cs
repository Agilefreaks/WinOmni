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
                _eventAggregator.Publish(getAllNotificationsTask.Result.Data);
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
            throw new NotImplementedException();
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            observable.Where(i => i.Provider == OmniMessageTypeEnum.Notification).Subscribe(this);
        }
    }
}