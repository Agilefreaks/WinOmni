namespace NotificationsTests.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using Notifications.Api.Resources.v1;
    using Notifications.Handlers;
    using Notifications.Models;
    using NUnit.Framework;
    using OmniCommon.Models;

    [TestFixture]
    public class NotificationHandlerTests
    {
        #region Fields

        private Mock<INotifications> _mockNotifications;

        private MoqMockingKernel _mockingKernel;

        private INotificationsHandler _notificationsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockNotifications = _mockingKernel.GetMock<INotifications>();
            _mockingKernel.Bind<INotificationsHandler>().To<NotificationsHandler>().InSingletonScope();

            _notificationsHandler = _mockingKernel.Get<INotificationsHandler>();
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Notification>>();
            var observable = new Subject<OmniMessage>();
            var notificationObserver = new Subject<Notification>();
            var notification = new Notification();

            _mockNotifications.Setup(m => m.Last()).Returns(notificationObserver);

            _notificationsHandler.Start(observable);
            _notificationsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));
            notificationObserver.OnNext(notification);

            observer.Verify(o => o.OnNext(notification), Times.Once);
        }

        [Test]
        public void WhenAClippingArrives_SubscribeOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Notification>>();
            var observable = new Subject<OmniMessage>();

            _notificationsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            observer.Verify(o => o.OnNext(It.IsAny<Notification>()), Times.Never);            
        }

        #endregion
    }
}