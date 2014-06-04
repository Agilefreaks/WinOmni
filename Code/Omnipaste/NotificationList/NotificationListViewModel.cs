namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Notifications.Handlers;
    using Notifications.Models;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel
    {
        #region Fields

        private readonly INotificationsHandler _notificationsHandler;

        private IDisposable _subscription;

        #endregion

        #region Constructors and Destructors

        public NotificationListViewModel(INotificationsHandler notificationsHandler)
        {
            _notificationsHandler = notificationsHandler;
            Notifications = new ObservableCollection<INotificationViewModel>();
        }

        #endregion

        #region Public Properties

        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

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

        public void OnNext(Notification notification)
        {
            notification.Title = string.Concat("Incoming call from ", notification.phone_number);

            Execute.OnUIThread(() => Notifications.Add(new NotificationViewModel(notification)));
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            _subscription = _notificationsHandler.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _subscription.Dispose();
        }

        #endregion
    }
}