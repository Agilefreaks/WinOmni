namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Events.Handlers;
    using Ninject;
    using OmniCommon.Helpers;
    using Omnipaste.ExtensionMethods;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.AllActive, INotificationListViewModel
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
            Notifications.CollectionChanged += NotificationsCollectionChanged;

            _eventsHandler = eventsHandler;
            _omniClipboardHandler = omniClipboardHandler;
        }

        #endregion

        #region Public Properties

        [Inject]
        public INotificationViewModelFactory NotificationViewModelFactory { get; set; }

        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void ShowWindow(
            IWindowManager windowManager,
            INotificationListViewModel notificationListViewModel)
        {
            windowManager.ShowPopup(
                notificationListViewModel,
                null,
                new Dictionary<string, object>
                    {
                        { "Placement", PlacementMode.Absolute },
                        { "HorizontalOffset", SystemParameters.WorkArea.Right },
                        { "VerticalOffset", SystemParameters.WorkArea.Top },
                        { "TopMost", true }
                    });
        }

        public void NotificationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                e.NewItems.Cast<IScreen>().ToList<IScreen>().ForEach(
                    i =>
                    {
                        ActivateItem(i);
                        i.Deactivated += OnNotificationDeactivated;
                    });
            }
        }

        #endregion

        #region Methods

        protected override void OnActivate()
        {
            base.OnActivate();

            CreateNotificationsFromIncomingEvents();
            CreateNotificationsFromIncomingClippings();
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

        private void CreateNotificationsFromIncomingClippings()
        {
            _clippingsSubscription =
                _omniClipboardHandler.Clippings.ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(
                        clipping => Notifications.Add(NotificationViewModelFactory.Create(clipping)));
        }

        private void CreateNotificationsFromIncomingEvents()
        {
            _notificationsSubscription =
                _eventsHandler.ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(
                        @event => Notifications.Add(NotificationViewModelFactory.Create(@event)));
        }

        private void OnNotificationDeactivated(object sender, DeactivationEventArgs e)
        {
            var notificationViewModel = (INotificationViewModel)sender;
            notificationViewModel.Deactivated -= OnNotificationDeactivated;
            Notifications.Remove(notificationViewModel);
        }

        #endregion
    }
}