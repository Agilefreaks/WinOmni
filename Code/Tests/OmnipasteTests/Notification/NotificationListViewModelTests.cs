namespace OmnipasteTests.Notification
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel;
    using Notifications.Handlers;
    using Notifications.Models;
    using NUnit.Framework;
    using Omnipaste.NotificationList;

    [TestFixture]
    public class NotificationListViewModelTests
    {
        #region Fields

        private Mock<INotificationsHandler> _mockNotificationHandler;

        private INotificationListViewModel _subject;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            var mockingKernel = new MockingKernel();

            _mockNotificationHandler = new Mock<INotificationsHandler>();
            mockingKernel.Bind<INotificationsHandler>().ToConstant(_mockNotificationHandler.Object);
            mockingKernel.Bind<INotificationListViewModel>().To<NotificationListViewModel>();

            _subject = mockingKernel.Get<INotificationListViewModel>();
        }

        [Test]
        public void Activate_WillSubscribeToHandler()
        {
            _subject.Activate();

            _mockNotificationHandler.Verify(nh => nh.Subscribe(_subject), Times.Once);
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
            var notification = new Notification();

            _subject.OnNext(notification);

            _subject.Notifications.First().Model.Should().Be(notification);
        }

        #endregion
    }
}