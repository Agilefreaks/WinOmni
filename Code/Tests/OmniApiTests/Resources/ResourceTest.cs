namespace OmniApiTests.Resources
{
    using System.Reactive;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;

    public class ResourceTest
    {
        private TestResource _subject;

        private Mock<IConfigurationService> _mockConfigurationService;

        public class TestResource : Resource<TestResource.ITestApi>
        {
            public interface ITestApi
            {
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockConfigurationService.SetupGet(m => m.AccessToken).Returns("AccessToken");
            _mockConfigurationService.SetupGet(m => m.RefreshToken).Returns("RefreshToken");

            _subject = new TestResource { ConfigurationService = _mockConfigurationService.Object };
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
        public void Authorize_Always_WrapsInAAuthorizationObserver()
        {
            _subject.Authorize(Observable.Empty<string>()).Should().BeOfType<AnonymousObservable<string>>();
        }
    }
}