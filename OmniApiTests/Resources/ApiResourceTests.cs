namespace OmniApiTests.Resources
{
    using FluentAssertions;
    using NUnit.Framework;
    using OmniApi.Resources;
    using RestSharp;

    [TestFixture]
    public class ApiResourceTests
    {
        private class TestResource : ApiResource
        {
        }

        [Test]
        public void BuildWillSetBaseUrlAndRestClient()
        {
            IRestClient restClient = new RestClient();
            TestResource subject = ApiResource.Build<TestResource>("http://test.omnipasteapp.com", restClient);

            subject.ApiUrl.Should().Be("http://test.omnipasteapp.com/v1");
            subject.RestClient.Should().Be(restClient);
        }
    }
}