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

        private Mock<IEventsHandler> _mockNotificationHandler;

        private INotificationListViewModel _subject;

        private Mock<IOmniClipboardHandler> _mockOmniClipboardHandler;

        private TestScheduler _testScheduler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            var mockingKernel = new MockingKernel();

            _mockNotificationHandler = new Mock<IEventsHandler>();
            mockingKernel.Bind<IEventsHandler>().ToConstant(_mockNotificationHandler.Object);
            mockingKernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();

            _mockOmniClipboardHandler = new Mock<IOmniClipboardHandler>{ DefaultValue = DefaultValue.Mock };
            mockingKernel.Bind<IOmniClipboardHandler>().ToConstant(_mockOmniClipboardHandler.Object);

            _subject = mockingKernel.Get<INotificationListViewModel>();

            _testScheduler = new TestScheduler();

            ITestableObservable<Clipping> testableObservable = _testScheduler.CreateHotObservable(
                new Recorded<Notification<Clipping>>(200, System.Reactive.Notification.CreateOnNext(new Clipping())));

            _mockOmniClipboardHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Clipping>>()))
                .Callback<IObserver<Clipping>>(o => testableObservable.Subscribe(o));
        }

        [Test]
        public void Activate_WillSubscribeToHandler()
        {
            _subject.Activate();

            _mockNotificationHandler.Verify(nh => nh.Subscribe(_subject), Times.Once);
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_AddsNewNotificationViewModel()
        {
            _subject.Activate();

            _testScheduler.Start();

            _subject.Notifications.Count.Should().Be(1);
        }

        [Test]
        public void WhenNewClippingComesThroughOmniClipboardHandler_CreatesNewNotificationViewModel()
        {
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
            _mockNotificationHandler.Setup(nh => nh.Subscribe(_subject)).Returns(mockDisposable.Object);

            _subject.Activate();
            _subject.Deactivate(true);

            mockDisposable.Verify(d => d.Dispose(), Times.Once);
        }

        [Test]
        public void OnNext_Always_AddsNotificationToCollection()
        {
            var notification = new Event();

            _subject.OnNext(notification);

            //_subject.Notifications.First().Model.Should().Be(notification);
        }

        #endregion
    }
}