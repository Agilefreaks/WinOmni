namespace Notifications.Handlers
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Notifications.Api;
    using Notifications.Api.Resources.v1;
    using Notifications.Models;
    using OmniCommon.Models;

    public class NotificationsHandler : INotificationsHandler
    {
        #region Fields

        private readonly Subject<Notification> _subject;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public NotificationsHandler(INotifications notifications)
        {
            _subject = new Subject<Notification>();
            Notifications = notifications;
        }

        #endregion

        #region Public Properties

        public INotifications Notifications { private get; set; }

        #endregion

        #region Public Methods and Operators

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
            Notifications.Last().Subscribe(
                // OnNext
                n => _subject.OnNext(n),
                // onError
                e => Debugger.Break());
        }

        public void Start(IObservable<OmniMessage> omniMessageObservable)
        {
            SubscribeTo(omniMessageObservable);
        }

        public void Stop()
        {
            _subscription.Dispose();
        }

        public IDisposable Subscribe(IObserver<Notification> observer)
        {
            return _subject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private void SubscribeTo(IObservable<OmniMessage> observable)
        {
            _subscription = observable.Where(i => i.Provider == OmniMessageTypeEnum.Notification).Subscribe(this);
        }

        #endregion
    }
}