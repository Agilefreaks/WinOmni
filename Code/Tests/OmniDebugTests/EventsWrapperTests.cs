namespace OmniDebugTests
{
    using System;
    using Events.Api.Resources.v1;
    using Events.Models;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniDebug.Services;

    [TestFixture]
    [Ignore("These tests should be rewritten once the circular dependency between OmniService and the authorization observer is resolved, allowing us to inject an "
            + "IEvents instance here as the dependency")]
    public class EventsWrapperTests
    {
        private EventsWrapper _subject;

        private Mock<Events> _mockEvents;

        [SetUp]
        public void Setup()
        {
            _mockEvents = new Mock<Events>();
            _subject = new EventsWrapper(_mockEvents.Object);
        }

        [Test]
        public void Last_AfterFirstCallingMockLast_ReturnsTheGivenEventObject()
        {
            var mockObserver = new Mock<IObserver<Event>>();
            var @event = new Event { Content = "test", Time = DateTime.Now, Type = EventTypeEnum.IncomingSmsEvent };

            _subject.MockLast(@event);
            _subject.Last().Subscribe(mockObserver.Object);

            mockObserver.Verify(x => x.OnNext(@event), Times.Once);
        }

        [Test]
        public void Last_CallingTwiceInARowAfterFirstCallingMockLast_ReturnsTheObservableFromTheGivenEventsObject()
        {
            var mockEventObservable = new Mock<IObservable<Event>>();
            _mockEvents.Setup(x => x.Last()).Returns(mockEventObservable.Object);

            _subject.MockLast(new Event());
            _subject.Last();
            _subject.Last().Should().Be(mockEventObservable.Object);
        }

        [Test]
        public void Last_WithoutFirstCallingMockLast_ReturnsTheObservableFromTheGivenEventsObject()
        {
            var mockEventObservable = new Mock<IObservable<Event>>();
            _mockEvents.Setup(x => x.Last()).Returns(mockEventObservable.Object);

            _subject.Last().Should().Be(mockEventObservable.Object);
        }
    }
}
