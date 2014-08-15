namespace OmnipasteTests.MasterEventList.IncomingSmsEventList
{
    using System;
    using System.Linq;
    using System.Reactive;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Framework;
    using Omnipaste.MasterEventList.IncomingSmsEventList;

    [TestFixture]
    public class IncomingSmsEventListViewModelTests
    {
        private MoqMockingKernel _kernel;

        private IncomingSmsEventListViewModel _subject;

        private Mock<IEventsHandler> _mockEventsHandler;

        private TestScheduler _testScheduler;

        private ITestableObservable<Event> _testableIncomingEventsObservable;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();

            SetupTestScheduler();

            _mockEventsHandler = _kernel.GetMock<IEventsHandler>();
            _mockEventsHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(o => _testableIncomingEventsObservable.Subscribe(o));
            _kernel.Bind<IEventsHandler>().ToConstant(_mockEventsHandler.Object);

            _subject = _kernel.Get<IncomingSmsEventListViewModel>();
        }

        [Test]
        public void IncomingSmsEvent_IsAddedToTheList()
        {
            _testScheduler.Start();

            _subject.Items.Count.Should().Be(1);
            _subject.Items.First().Type.Should().Be(EventTypeEnum.IncomingSmsEvent);
        }

        private void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();

            _testableIncomingEventsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event { PhoneNumber = "your number", Type = EventTypeEnum.IncomingCallEvent })),
                    new Recorded<Notification<Event>>(200, Notification.CreateOnNext(new Event { PhoneNumber = "your number", Type = EventTypeEnum.IncomingSmsEvent })));

            SchedulerProvider.Dispatcher = _testScheduler;
        }

    }
}