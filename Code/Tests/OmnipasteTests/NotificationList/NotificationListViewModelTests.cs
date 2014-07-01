namespace OmnipasteTests.NotificationList
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Clipboard.Handlers;
    using Clipboard.Models;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using NUnit.Framework;
    using Omnipaste.Notification;
    using Omnipaste.NotificationList;

    [TestFixture]
    public class NotificationListViewModelTests
    {
        #region Fields

        private Mock<IEventsHandler> _mockEventsHandler;

        private INotificationListViewModel _subject;

        private Mock<IOmniClipboardHandler> _mockOmniClipboardHandler;

        private TestScheduler _testScheduler;

        private ITestableObservable<Clipping> _testableClippingsObservable;

        private ITestableObservable<Event> _testableEventsObservable;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            var mockingKernel = new MockingKernel();

            _mockEventsHandler = new Mock<IEventsHandler>();
            mockingKernel.Bind<IEventsHandler>().ToConstant(_mockEventsHandler.Object);

            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler>{ DefaultValue = DefaultValue.Mock };
            mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);

            mockingKernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();
            _subject = mockingKernel.Get<INotificationListViewModel>();

            _testScheduler = new TestScheduler();
            _testableClippingsObservable =
                _testScheduler.CreateHotObservable(
                    new Recorded<Notification<Clipping>>(200, Notification.CreateOnNext(new Clipping())));
            _testableEventsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event())));
        }

        [Test]
        public void NewEventArrives_AddsNewNotificationViewModel()
        {
            _mockEventsHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(o => _testableEventsObservable.Subscribe(o));
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Type.Should().Be(NotificationViewModelTypeEnum.IncomingCall);
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_AddsNewNotificationViewModel()
        {
            _mockOmniClipboardHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Callback<IObserver<Clipping>>(o => _testableClippingsObservable.Subscribe(o));
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Count.Should().Be(1);
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_CreatesNewNotificationViewModel()
        {
            _mockOmniClipboardHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Callback<IObserver<Clipping>>(o => _testableClippingsObservable.Subscribe(o));
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.First().Type.Should().Be(NotificationViewModelTypeEnum.Clipping);
        }

        [Test]
        public void Constructor_WillInitializeNotificationsCollection()
        {
            _subject.Notifications.Should().NotBeNull();
        }

        [Test]
        public void Deativate_WillUnsubscribeFromHandler()
        {
            var mockDisposable = new Mock<IDisposable>();
            _mockEventsHandler.Setup(nh => nh.Subscribe(_subject)).Returns(mockDisposable.Object);

            _subject.Activate();
            _subject.Deactivate(true);

            mockDisposable.Verify(d => d.Dispose(), Times.Once);
        }

        #endregion
    }
}