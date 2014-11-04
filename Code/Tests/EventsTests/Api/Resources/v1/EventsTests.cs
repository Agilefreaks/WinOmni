namespace EventsTests.Api.Resources.v1
{
    using System;
    using Events.Api.Resources.v1;
    using Events.Models;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class EventsTests
    {
        private Events _subject;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<IOAuth2> _mockOAuth2;

        [SetUp]
        public void SetUp()
        {
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.SetupGet(cs => cs.AccessToken).Returns("access token");

            _mockWebProxyFactory = new Mock<IWebProxyFactory>();
            _mockOAuth2 = new Mock<IOAuth2>();
            _subject = new Events(mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               OAuth2 = _mockOAuth2.Object
                           };
        }

        [Test]
        public void Last_Always_ReturnsAnObservable()
        {
            _subject.Last().Should().BeAssignableTo<IObservable<Event>>();
        }
    }
}