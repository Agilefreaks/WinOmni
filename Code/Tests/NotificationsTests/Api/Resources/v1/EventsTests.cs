namespace NotificationsTests.Api.Resources.v1
{
    using System;
    using Events.Api.Resources.v1;
    using Events.Models;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class EventsTests
    {
        private Events _subject;

        [SetUp]
        public void SetUp()
        {
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.SetupGet(cs => cs.AccessToken).Returns("access token");

            _subject = new Events() { ConfigurationService = mockConfigurationService.Object };
        }

        [Test]
        public void Last_Always_ReturnsAnObservable()
        {
            _subject.Last().Should().BeAssignableTo<IObservable<Event>>();
        }
    }
}