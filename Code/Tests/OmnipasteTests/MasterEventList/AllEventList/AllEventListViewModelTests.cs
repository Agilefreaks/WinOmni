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
    using Omnipaste;
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

            _subject = _kernel.Get<AllEventListViewModel>();
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

        [Test]
        public void NewInstance_SubscribesToEventHandler()
        {
            _mockEventsHandler.Verify(h => h.Subscribe(It.IsAny<IObserver<Event>>()));
        }

        [Test]
        public void IncomingEvents_NoMatterTheType_AreAddedToTheList()
        {
            _testScheduler.Start();

            _subject.IncomingEvents.Count.Should().Be(2);
        }

        [Test]
        public void Status_WhenEventsListIsEmpty_StatusIsEmpty()
        {
            _subject.Status.Should().Be(ListViewModelStatusEnum.Empty);
        }

        [Test]
        public void Status_WhenEventListIsNotEmpty_StatusIsNotEmpty()
        {
            _testScheduler.Start();

            _subject.Status.Should().Be(ListViewModelStatusEnum.NotEmpty);
        }
    }
}