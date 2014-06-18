namespace NotificationsTests.Handlers
{
    using System;
    using System.Net;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using Notifications.API;
    using Notifications.Handlers;
    using Notifications.Models;
    using NUnit.Framework;
    using OmniCommon.Models;
    using RestSharp;

    [TestFixture]
    public class NotificationHandlerTests
    {
        #region Fields

        private Mock<INotificationsApi> _mockNotificationApi;

        private MoqMockingKernel _mockingKernel;

        private INotificationsHandler _notificationsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockNotificationApi = _mockingKernel.GetMock<INotificationsApi>();
            _mockingKernel.Bind<INotificationsApi>().ToConstant(_mockNotificationApi.Object);
            _mockingKernel.Bind<INotificationsHandler>().To<NotificationsHandler>().InSingletonScope();

            _notificationsHandler = _mockingKernel.Get<INotificationsHandler>();
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Notification>>();
            var observable = new Subject<OmniMessage>();
            var notification = new Notification();

            _mockNotificationApi.Setup(na => na.Last())
                .ReturnsAsync(new RestResponse<Notification> { Data = notification, StatusCode = HttpStatusCode.OK });

            _notificationsHandler.Start(observable);
            _notificationsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));

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