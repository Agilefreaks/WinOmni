namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Api
{
    using FluentAssertions;
    using NUnit.Framework;
    using Omnipaste.OmniClipboard.Infrastructure.Api.Resources;

    [TestFixture]
    public class ClippingsTests
    {
        private Clippings _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new Clippings("http://test.com", "v1", "apiKey");
        }

        [Test]
        public void ApiUrl_Always_ReturnsApiUrlWithVersion()
        {
            _subject.ApiUrl.Should().Be("http://test.com/v1");
        }
    }
}
