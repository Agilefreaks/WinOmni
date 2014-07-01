namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Notifications.Handlers;
    using Notifications.Models;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel
    {
        #region Fields

        private readonly INotificationsHandler _notificationsHandler;

        private readonly IOmniClipboardHandler _omniClipboardHandler;

        private IDisposable _notificationsSubscription;

        private IDisposable _clippingsSubscription;

        #endregion

        #region Constructors and Destructors

        public NotificationListViewModel(INotificationsHandler notificationsHandler, IOmniClipboardHandler omniClipboardHandler)
        {
            Notifications = new ObservableCollection<INotificationViewModel>();

            _notificationsHandler = notificationsHandler;
            _omniClipboardHandler = omniClipboardHandler;
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
            Execute.OnUIThread(() => Notifications.Add(NotificationViewModelBuilder.Build(notification)));
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            _notificationsSubscription = _notificationsHandler.Subscribe(this);

            _clippingsSubscription = _omniClipboardHandler.Subscribe(
                c => Execute.OnUIThread(() => Notifications.Add(NotificationViewModelBuilder.Build(c))),
                e => { });
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (_notificationsSubscription != null)
            {
                _notificationsSubscription.Dispose();
            }

            if (_clippingsSubscription != null)
            {
                _clippingsSubscription.Dispose();
            }
        }

        #endregion
    }
}