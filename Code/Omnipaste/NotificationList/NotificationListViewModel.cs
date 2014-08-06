namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Ninject;
    using Omnipaste.Framework;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel
    {
        #region Fields

        private readonly IEventsHandler _eventsHandler;

        private readonly IOmniClipboardHandler _omniClipboardHandler;

        private IDisposable _clippingsSubscription;

        private IDisposable _notificationsSubscription;

        #endregion

        #region Constructors and Destructors

        public NotificationListViewModel(IEventsHandler eventsHandler, IOmniClipboardHandler omniClipboardHandler)
        {
            Notifications = new ObservableCollection<INotificationViewModel>();

            _eventsHandler = eventsHandler;
            _omniClipboardHandler = omniClipboardHandler;
        }

        #endregion

        #region Public Properties

        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        [Inject]
        public INotificationViewModelFactory NotificationViewModelFactory { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void ShowWindow(IWindowManager windowManager, INotificationListViewModel notificationListViewModel)
        {
            windowManager.ShowWindow(
                notificationListViewModel,
                null,
                new Dictionary<string, object>
                    {
                        { "Height", SystemParameters.WorkArea.Height },
                        { "Width", SystemParameters.WorkArea.Width }
                    });
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            _notificationsSubscription = _eventsHandler
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(
                    notification => Notifications.Add(NotificationViewModelFactory.Create(notification)),
                    exception => { });

            _clippingsSubscription = _omniClipboardHandler
                .ObserveOn(SchedulerProvider.Dispatcher)
                .Subscribe(
                    clipping => Notifications.Add(NotificationViewModelFactory.Create(clipping)),
                    exception => { });
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