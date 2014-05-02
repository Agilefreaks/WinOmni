using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using OmniCommon.Interfaces;
using OmniCommon.Models;
using RestSharp;

namespace Notifications
{
    public class NotificationsHandler : IOmniMessageHandler
    {
        public INotificationsAPI NotificationsAPI { get; set; }

        public NotificationsHandler(INotificationsAPI notificationsAPI)
        {
            NotificationsAPI = notificationsAPI;
        }

        public void OnNext(OmniMessage value)
        {
            var getAllNotificationsTask = NotificationsAPI.GetAll();
            getAllNotificationsTask.Wait();
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
            observable.Where(i => i.Provider == "notification").Subscribe(this);
        }
    }
}