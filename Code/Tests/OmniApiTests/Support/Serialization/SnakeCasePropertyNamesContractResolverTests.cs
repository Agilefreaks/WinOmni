namespace OmniApiTests.Support.Serialization
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Support.Serialization;

    [TestFixture]
    public class SnakeCasePropertyNamesContractResolverTests
    {
        private SnakeCasePropertyNamesContractResolver _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new SnakeCasePropertyNamesContractResolver();
        }

        [Test]
        public void ResolvePropertyName_ForOneWordUpperCase()
        {
            _subject.GetResolvedPropertyName("Id").Should().Be("id");
        }

        [Test]
        public void ResolvePropertyName_ForTwoWordsCamelCase()
        {
            _subject.GetResolvedPropertyName("AccessToken").Should().Be("access_token");
        }
    }
}