namespace EventsTests.Handlers
{
    using System;
    using System.Reactive.Linq;
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

        private Mock<IEvents> _mockEvents;

        private MoqMockingKernel _mockingKernel;

        private IEventsHandler _eventsHandler;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void Setup()
        {
            _mockingKernel = new MoqMockingKernel();

            _mockEvents = _mockingKernel.GetMock<IEvents>();
            _mockingKernel.Bind<IEventsHandler>().To<EventsHandler>().InSingletonScope();

            _eventsHandler = _mockingKernel.Get<IEventsHandler>();
        }

        [Test]
        public void WhenANotificationMessageArrives_SubscriberOnNextIsCalled()
        {
            var observer = new Mock<IObserver<Event>>();
            var omniMessageObservable = new Subject<OmniMessage>();
            var @event = new Event();
            var eventObservable = Observable.Return(@event);

            _mockEvents.Setup(m => m.Last()).Returns(eventObservable);

            _eventsHandler.Start(omniMessageObservable);
            _eventsHandler.Subscribe(observer.Object);

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageTypeEnum.Notification));

            observer.Verify(o => o.OnNext(@event), Times.Once);
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