﻿namespace Omnipaste.NotificationList
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
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.AllActive, INotificationListViewModel
    {
        #region Constants

        private const int NotificationWindowWidth = 385;

        #endregion

        #region Fields

        private readonly IEventsHandler _eventsHandler;

        private readonly IOmniClipboardHandler _omniClipboardHandler;

        private IDisposable _clippingsSubscription;

        private double _height;

        private IDisposable _notificationsSubscription;

        #endregion

        #region Constructors and Destructors

        public NotificationListViewModel(IEventsHandler eventsHandler, IOmniClipboardHandler omniClipboardHandler)
        {
            Notifications = new ObservableCollection<INotificationViewModel>();
            Notifications.CollectionChanged += NotificationsCollectionChanged;

            _eventsHandler = eventsHandler;
            _omniClipboardHandler = omniClipboardHandler;

            Height = double.NaN;
        }

        #endregion

        #region Public Properties

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value.Equals(_height))
                {
                    return;
                }
                _height = value;
                NotifyOfPropertyChange();
            }
        }

        [Inject]
        public INotificationViewModelFactory NotificationViewModelFactory { get; set; }

        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        [Inject]
        public IWindowManager WindowManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Show()
        {
            Height = SystemParameters.WorkArea.Height;
            WindowManager.ShowPopup(
                this,
                null,
                new Dictionary<string, object>
                    {
                        { "Placement", PlacementMode.Absolute },
                        { "HorizontalOffset", SystemParameters.WorkArea.Right - NotificationWindowWidth },
                        { "VerticalOffset", SystemParameters.WorkArea.Top },
                        { "TopMost", true }
                    });
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
                _notificationsSubscription = null;
            }

            if (_clippingsSubscription != null)
            {
                _clippingsSubscription.Dispose();
                _clippingsSubscription = null;
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
                    .SubscribeAndHandleErrors(@event => Notifications.Add(NotificationViewModelFactory.Create(@event)));
        }

        private void NotificationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        private void OnNotificationDeactivated(object sender, DeactivationEventArgs e)
        {
            var notificationViewModel = (INotificationViewModel)sender;
            notificationViewModel.Deactivated -= OnNotificationDeactivated;
            Notifications.Remove(notificationViewModel);
        }

        #endregion
    }
}