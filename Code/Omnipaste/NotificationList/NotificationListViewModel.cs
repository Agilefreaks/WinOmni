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
    using Castle.Core.Internal;
    using Clipboard.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Helpers;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Models;
    using Omnipaste.Notification;
    using Omnipaste.Services.Repositories;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.AllActive, INotificationListViewModel
    {
        #region Constants

        private const int NotificationWindowWidth = 385;

        #endregion

        #region Fields

        private readonly IClippingRepository _clippingRepository;

        private readonly IPhoneCallRepository _phoneCallRepository;

        private readonly IMessageRepository _messageRepository;

        private double _height;

        private IDisposable _clippingsSubscription;

        private IDisposable _callSubscription;

        private IDisposable _messageSubscription;

        #endregion

        #region Constructors and Destructors

        public NotificationListViewModel(IClippingRepository clippingRepository, IPhoneCallRepository phoneCallRepository, IMessageRepository messageRepository, IEventAggregator eventAggregator)
        {
            Notifications = new ObservableCollection<INotificationViewModel>();
            Notifications.CollectionChanged += NotificationsCollectionChanged;

            _clippingRepository = clippingRepository;
            _phoneCallRepository = phoneCallRepository;
            _messageRepository = messageRepository;

            Height = double.NaN;

            eventAggregator.Subscribe(this);
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

            CreateNotificationsFromIncomingClippings();
            CreateNotificationsFromIncomingMessages();
            CreateNotificationsFromIncomingCalls();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (_clippingsSubscription != null)
            {
                _clippingsSubscription.Dispose();
                _clippingsSubscription = null;
            }

            if (_callSubscription != null)
            {
                _callSubscription.Dispose();
                _callSubscription = null;
            }

            if (_messageSubscription != null)
            {
                _messageSubscription.Dispose();
                _messageSubscription = null;
            }
        }

        private void CreateNotificationsFromIncomingClippings()
        {
            _clippingsSubscription =
                _clippingRepository.OperationObservable.Changed()
                    .Select(o => o.Item)
                    .Where(item => item.Source == Clipping.ClippingSourceEnum.Cloud && item.WasViewed == false)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(
                        clipping => Notifications.Add(NotificationViewModelFactory.Create(clipping)));
        }

        private void CreateNotificationsFromIncomingMessages()
        {
            _messageSubscription =
                _messageRepository.OperationObservable.Changed()
                    .Select(o => o.Item)
                    .Where(item => item.Source == SourceType.Remote && item.WasViewed == false)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(
                        message => Notifications.Add(NotificationViewModelFactory.Create(message)));
        }

        private void CreateNotificationsFromIncomingCalls()
        {
            _callSubscription =
                _phoneCallRepository.OperationObservable.Changed()
                    .Select(o => o.Item)
                    .Where(item => item.Source == SourceType.Remote && item.WasViewed == false)
                    .ObserveOn(SchedulerProvider.Dispatcher)
                    .SubscribeAndHandleErrors(call => Notifications.Add(NotificationViewModelFactory.Create(call)));
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

        public void Handle(DismissNotification message)
        {
            Notifications.Where(n => n.Identifier.Equals(message.Identifier)).ForEach(n => n.Dismiss());
        }
    }
}