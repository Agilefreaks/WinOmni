namespace OmnipasteTests.NotificationList
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Caliburn.Micro;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using OmniCommon.Interfaces;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Notification;
    using Omnipaste.Notification.ClippingNotification;
    using Omnipaste.Notification.IncomingCallNotification;
    using Omnipaste.NotificationList;

    [TestFixture]
    public class NotificationListViewModelTests
    {
        #region Fields

        private Mock<IEventsHandler> _mockEventsHandler;

        private Mock<IOmniClipboardHandler> _mockOmniClipboardHandler;

        private MoqMockingKernel _mockingKernel;

        private INotificationListViewModel _subject;

        private TestScheduler _testScheduler;

        private ITestableObservable<Clipping> _testableClippingsObservable;

        private ITestableObservable<Event> _testableEventsObservable;

        private Mock<INotificationViewModelFactory> _mockNotificationViewModelFactory;

        private Mock<IApplicationService> _mockApplicationService;

        private Mock<IEventAggregator> _mockEventAggregator;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockingKernel.Bind<IDisposable>().ToMock();

            SetupTestScheduler();

            SetupEventsHandler();

            SetupClipboardHandler();

            _mockingKernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();
            _mockApplicationService = _mockingKernel.GetMock<IApplicationService>();
            _mockingKernel.Bind<IApplicationService>().ToConstant(_mockApplicationService.Object);
            _mockNotificationViewModelFactory = _mockingKernel.GetMock<INotificationViewModelFactory>();
            _mockingKernel.Bind<INotificationViewModelFactory>().ToConstant(_mockNotificationViewModelFactory.Object);
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockingKernel.Bind<IEventAggregator>().ToConstant(_mockEventAggregator.Object);
            
            _subject = _mockingKernel.Get<INotificationListViewModel>();
        }

        [Test]
        public void Constructor_WillInitializeNotificationsCollection()
        {
            _subject.Notifications.Should().NotBeNull();
        }

        [Test]
        public void Constructor_WillSubscribeInstanceToEventAggregator()
        {
            _mockEventAggregator.Verify(m => m.Subscribe(_subject));
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_AddsNewNotificationViewModel()
        {
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<Clipping>()))
                .Returns(new Mock<IClippingNotificationViewModel>().Object);
            _mockOmniClipboardHandler.Setup(h => h.Clippings).Returns(_testableClippingsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Count.Should().Be(1);
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_CreatesNewNotificationViewModel()
        {
            var mockClippingNotificationViewModel = new Mock<IClippingNotificationViewModel>();
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<Clipping>()))
                .Returns(mockClippingNotificationViewModel.Object);
            _mockOmniClipboardHandler.Setup(h => h.Clippings).Returns(_testableClippingsObservable);
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Should().Be(mockClippingNotificationViewModel.Object);
        }

        [Test]
        public void WhenAnIncomingCallComesThroughOmniEventsHandler_CreatesNewNotificationViewModel()
        {
            var mockIncomingCallNotificationViewModel = new Mock<IIncomingCallNotificationViewModel>();
            _mockNotificationViewModelFactory.Setup(f => f.Create(It.IsAny<Event>()))
                .Returns(mockIncomingCallNotificationViewModel.Object);
            _mockEventsHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(o => _testableEventsObservable.Subscribe(o));
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Should().Be(mockIncomingCallNotificationViewModel.Object);
        }

        [Test]
        public void HandleWithDismissNotification_WhenNotificationExist_DismissesNotification()
        {
            const int Identifier = 42;
            var mockNotification = new Mock<INotificationViewModel> { DefaultValue = DefaultValue.Mock };
            mockNotification.SetupGet(m => m.Identifier).Returns(Identifier);
            _subject.Notifications.Add(mockNotification.Object);

            _subject.Handle(new DismissNotification(Identifier));

            mockNotification.Verify(m => m.Dismiss());
        }

        [Test]
        public void HandleWithDismissNotification_WhenNotificationDoesNotExist_DoesNotDismissOtherNotification()
        {
            var mockNotification = new Mock<INotificationViewModel> { DefaultValue = DefaultValue.Mock };
            mockNotification.SetupGet(m => m.Identifier).Returns(111);
            _subject.Notifications.Add(mockNotification.Object);

            _subject.Handle(new DismissNotification(42));

            mockNotification.Verify(m => m.Dismiss(), Times.Never());
        }

        #endregion

        #region Methods

        private void SetupClipboardHandler()
        {
            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);
        }

        private void SetupEventsHandler()
        {
            _mockEventsHandler = new Mock<IEventsHandler> { DefaultValue = DefaultValue.Mock };
            _mockingKernel.Bind<IEventsHandler>().ToConstant(_mockEventsHandler.Object);
        }

        private void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();
            _testableClippingsObservable =
                _testScheduler.CreateHotObservable(
                    new Recorded<Notification<Clipping>>(200, Notification.CreateOnNext(new Clipping())));

            _testScheduler.CreateHotObservable(
                new Recorded<Notification<Event>>(
                    300,
                    Notification.CreateOnNext(new Event { PhoneNumber = "phone number" })));
            _testableEventsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event {PhoneNumber = "your number"})));

            SchedulerProvider.Dispatcher = _testScheduler;
        }

        #endregion
    }
}