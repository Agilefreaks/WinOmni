namespace NotificationsTests.Handlers
{
    using System;
    using System.Reactive.Subjects;
    using Events.Api.Resources.v1;
    using Events.Handlers;
    using Events.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Models;

    [TestFixture]
    public class EventHandlerTests
    {
        #region Fields

        private Mock<IEvents> _mockNotifications;

        private MoqMockingKernel _mockingKernel;

        private IEventsHandler _eventsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockNotifications = _mockingKernel.GetMock<IEvents>();
            _mockingKernel.Bind<IEventsHandler>().To<EventsHandler>().InSingletonScope();

            _eventsHandler = _mockingKernel.Get<IEventsHandler>();
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Event>>();
            var observable = new Subject<OmniMessage>();
            var notificationObserver = new Subject<Event>();
            var notification = new Event();

            _mockNotifications.Setup(m => m.Last()).Returns(notificationObserver);

            _eventsHandler.Start(observable);
            _eventsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));
            notificationObserver.OnNext(notification);

            observer.Verify(o => o.OnNext(notification), Times.Once);
        }

        [Test]
        public void WhenAClippingArrives_SubscribeOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Event>>();
            var observable = new Subject<OmniMessage>();

            _eventsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageTypeEnum.Clipboard));

            observer.Verify(o => o.OnNext(It.IsAny<Event>()), Times.Never);            
        }

        #endregion
    }
}