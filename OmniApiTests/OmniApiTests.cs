namespace OmniApiTests
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi;

    [TestFixture]
    public class OmniApiTests
    {
        [Test]
        public void ApiUrlShouldBeBasedOnBaseUrl()
        {
            OmniApi.BaseUrl = "http://test.omnipasteapp.com";

            OmniApi.ApiUrl.Should().Be("http://test.omnipasteapp.com/api/v1");
        }
    }
}