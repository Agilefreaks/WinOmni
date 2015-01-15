﻿namespace EventsTests.Handlers
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using Events.Api.Resources.v1;
    using Events.Handlers;
    using Events.Models;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
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
            _mockEvents.Setup(m => m.Last()).Returns(Observable.Return(@event));
            _eventsHandler.Start(omniMessageObservable);
            _eventsHandler.Subscribe(observer.Object);
            DispatcherProvider.Instance = new ImmediateDispatcherProvider();
            var autoResetEvent = new AutoResetEvent(false);
            observer.Setup(o => o.OnNext(@event)).Callback(() => autoResetEvent.Set());

            omniMessageObservable.OnNext(new OmniMessage(OmniMessageProviderEnum.Notification));

            autoResetEvent.WaitOne(1000);
            observer.Verify(o => o.OnNext(@event), Times.Once);
        }

        [Test]
        public void WhenAClippingArrives_SubscribeOnNextIsNotCalled()
        {
            var observer = new Mock<IObserver<Event>>();
            var observable = new Subject<OmniMessage>();

            _eventsHandler.Subscribe(observer.Object);

            observable.OnNext(new OmniMessage(OmniMessageProviderEnum.Clipboard));

            observer.Verify(o => o.OnNext(It.IsAny<Event>()), Times.Never);            
        }

        #endregion
    }
}