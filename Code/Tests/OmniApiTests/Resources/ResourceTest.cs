namespace OmniApiTests.Resources
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class ResourceTest
    {
        private TestResource _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        private Mock<IWebProxyFactory> _mockWebProxyFactory;

        private Mock<IOAuth2> _mockOAuth2;

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService> { DefaultValue = DefaultValue.Mock };
            _mockConfigurationService.SetupGet(m => m.AccessToken).Returns("AccessToken");
            _mockConfigurationService.SetupGet(m => m.RefreshToken).Returns("RefreshToken");
            _mockConfigurationService.SetupGet(m => m[ConfigurationProperties.BaseUrl]).Returns("http://test.com");

            _mockWebProxyFactory = new Mock<IWebProxyFactory>();

            _mockOAuth2 = new Mock<IOAuth2>();
            _subject = new TestResource(_mockConfigurationService.Object, _mockWebProxyFactory.Object)
                           {
                               OAuth2 = _mockOAuth2.Object
                           };
        }

        [Test]
        public void Token_Always_GetsReadFromConfigurationService()
        {
            _subject.Token.ShouldBeEquivalentTo(new Token("AccessToken", "RefreshToken"));
        }

        [Test]
        public void AccessToken_Always_PrefixesWithBearer()
        {
            _subject.AccessToken.Should().Be("bearer AccessToken");
        }

        [Test]
        public void Authorize_Always_ReturnsAnObservableOfTheSameType()
        {
            _subject.Authorize(Observable.Empty<string>()).Should().BeAssignableTo<IObservable<string>>();
        }
    }
}