using System;
using System.Reactive.Linq;
using OmniCommon.Interfaces;
using OmniCommon.Models;

namespace Notifications
{
    public class NotificationsHandler : IOmniMessageHandler
    {
        public void OnNext(OmniMessage value)
        {
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