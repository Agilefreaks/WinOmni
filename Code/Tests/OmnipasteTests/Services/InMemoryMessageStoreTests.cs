namespace OmnipasteTests.Services
{
    using System;
    using System.Reactive;
    using Events.Handlers;
    using Events.Models;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Helpers;
    using Omnipaste.Models;
    using Omnipaste.Services;

    [TestFixture]
    public class InMemoryMessageStoreTests
    {
        private InMemoryMessageStore _subject;

        private Mock<IEventsHandler> _mockEventsHandler;

        private TestScheduler _testScheduler;

        [SetUp]
        public void Setup()
        {
            _mockEventsHandler = new Mock<IEventsHandler>();
            _subject = new InMemoryMessageStore(_mockEventsHandler.Object);
            _testScheduler = new TestScheduler();
            SchedulerProvider.Default = _testScheduler;
        }

        [TearDown]
        public void TearDown()
        {
            SchedulerProvider.Default = null;
        }

        [Test]
        public void AddMessage_Always_AddAnEntryInMessagesForTheGivenPhoneNumber()
        {
            var message = new Message { ContactInfo = new ContactInfo { Phone = "somePhone" } };

            _subject.AddMessage(message);

            _subject.Messages["somePhone"].Count.Should().Be(1);
            _subject.Messages["somePhone"][0].Should().Be(message);
        }

        [Test]
        public void AddMessage_PhoneIsNull_AddsTheMessageToTheStringEmptyEntry()
        {
            var message = new Message { ContactInfo = new ContactInfo { Phone = null } };

            _subject.AddMessage(message);

            _subject.Messages[string.Empty].Count.Should().Be(1);
            _subject.Messages[string.Empty][0].Should().Be(message);
        }

        [Test]
        public void EventsOccur_AfterStart_AddsAMessageBasedOnTheEventForIncommingSMSEvents()
        {
            const string PhoneNumber = "1231";
            var smsEvent = new Event { PhoneNumber = PhoneNumber, Type = EventTypeEnum.IncomingSmsEvent, Content = "test" };
            var callEvent = new Event { Type = EventTypeEnum.IncomingCallEvent };
            var eventsObservable = _testScheduler.CreateColdObservable(
                new Recorded<Notification<Event>>(100, Notification.CreateOnNext(smsEvent)),
                new Recorded<Notification<Event>>(200, Notification.CreateOnNext(callEvent)),
                new Recorded<Notification<Event>>(300, Notification.CreateOnCompleted<Event>()));
            _mockEventsHandler.Setup(x => x.Subscribe(It.IsAny<IObserver<Event>>()))
                .Callback<IObserver<Event>>(observer => eventsObservable.Subscribe(observer));

            _subject.Start();
            _testScheduler.Start();

            _subject.Messages[PhoneNumber].Count.Should().Be(1);
            _subject.Messages[PhoneNumber][0].Content.Should().Be("test");
            _subject.Messages[PhoneNumber][0].ContactInfo.Phone.Should().Be(PhoneNumber);
        }
    }
}