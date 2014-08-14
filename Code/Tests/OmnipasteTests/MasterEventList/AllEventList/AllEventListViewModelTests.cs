namespace OmnipasteTests.MasterEventList.AllEventList
{
    using System;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Ninject;
    using Ninject.MockingKernel.Moq;
    using NUnit.Framework;
    using Omnipaste.Framework;
    using Omnipaste.MasterEventList.AllEventList;
    using System.Reactive;

    [TestFixture]
    public class AllEventListViewModelTests
    {
        private MoqMockingKernel _kernel;

        private AllEventListViewModel _subject;

        private Mock<IEventsHandler> _mockEventsHandler;

        private TestScheduler _testScheduler;

        private ITestableObservable<Event> _testableIncomingCallObservable;

        private ITestableObservable<Event> _testableSmsObservable;

        [SetUp]
        public void SetUp()
        {
            _kernel = new MoqMockingKernel();

            SetupTestScheduler();
            
            _mockEventsHandler = _kernel.GetMock<IEventsHandler>();
            _mockEventsHandler
                .Setup(h => h.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(o => _testableSmsObservable.Subscribe(o));
            _kernel.Bind<IEventsHandler>().ToConstant(_mockEventsHandler.Object);

            _subject = _kernel.Get<AllEventListViewModel>();
        }

        private void SetupTestScheduler()
        {
            _testScheduler = new TestScheduler();

            _testableIncomingCallObservable =
                _testScheduler.CreateHotObservable(
                    new Recorded<Notification<Event>>(
                        300,
                        Notification.CreateOnNext(new Event { PhoneNumber = "phone number" })));
            _testableSmsObservable =
                _testScheduler.CreateColdObservable(
                    new Recorded<Notification<Event>>(100, Notification.CreateOnNext(new Event { PhoneNumber = "your number" })));

            SchedulerProvider.Dispatcher = _testScheduler;
        }

        [Test]
        public void NewInstance_SubscribesToEventHandler()
        {
            _mockEventsHandler.Verify(h => h.Subscribe(It.IsAny<IObserver<Event>>()));
        }

        [Test]
        public void IncomingEvent_AddsItToTheList()
        {
            _testScheduler.Start();

            _subject.IncomingEvents.Count.Should().Be(1);
        }
    }
}