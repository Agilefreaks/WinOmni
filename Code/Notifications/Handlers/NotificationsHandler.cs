namespace Notifications.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Notifications.API;
    using Notifications.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public class NotificationsHandler : INotificationsHandler, IOmniMessageHandler
    {
        #region Fields

        private readonly Subject<Notification> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public NotificationsHandler(INotificationsApi notificationsApi)
        {
            _subject = new Subject<Notification>();
            NotificationsApi = notificationsApi;
        }

        #endregion

        #region Public Properties

        public INotificationsApi NotificationsApi { private get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(OmniMessage value)
        {
            var getAllNotificationsTask = NotificationsApi.Last();
            getAllNotificationsTask.Wait();

            if (getAllNotificationsTask.Result.StatusCode == HttpStatusCode.OK)
            {
                _subject.OnNext(getAllNotificationsTask.Result.Data);
            }
        }

        public IDisposable Subscribe(IObserver<Notification> observer)
        {
            return _subject.Subscribe(observer);
        }

        public void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(i => i.Provider == OmniMessageTypeEnum.Notification).Subscribe(this);
        }

        #endregion
    }
}