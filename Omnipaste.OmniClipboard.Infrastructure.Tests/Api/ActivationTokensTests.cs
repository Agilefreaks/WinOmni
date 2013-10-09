namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Api
{
    using System.Collections.Specialized;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Infrastructure.Api.Resources;
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using RestSharp;

    [TestFixture]
    public class ActivationTokensTests
    {
        private Mock<IRestClient> _mockRestClient;

        private ActivationTokens _activationTokens;

        private Mock<IConfigurationManager> _mockConfigurationManager;

        [SetUp]
        public void SetUp()
        {
            _mockRestClient = new Mock<IRestClient>();
            _mockConfigurationManager = new Mock<IConfigurationManager>();
            _mockConfigurationManager.SetupGet(cm => cm.AppSettings).Returns(new NameValueCollection { { "apiUrl", "http://localhost/api" } });
            _activationTokens = new ActivationTokens(_mockConfigurationManager.Object, _mockRestClient.Object);

            _mockRestClient.Setup(rc => rc.Execute<ActivationData>(It.IsAny<IRestRequest>()))
                           .Returns(new RestResponse<ActivationData>());
        }

        [Test]
        public void Ctor_Always_HasARestClient()
        {
            Assert.AreSame(_activationTokens.RestClient, _mockRestClient.Object);
        }

        [Test]
        public void Ctor_Always_SetsTheCorrectUrlOnTheRestClient()
        {
            _mockRestClient.VerifySet(rc => rc.BaseUrl = "http://localhost/api/v1");
        }

        [Test]
        public void ResourceKey_Always_IsCorrect()
        {
            Assert.AreEqual(_activationTokens.ResourceKey, "activation_tokens");
        }

        [Test]
        public void Activate_Always_MakesaPUTRequest()
        {
            _activationTokens.Activate(string.Empty);

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Method == Method.PUT)));
        }

        [Test]
        public void Activate_Always_MakesARequestToTheCorrectResource()
        {
            _activationTokens.Activate(string.Empty);

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Resource == "activation_tokens")));
        }

        [Test]
        public void Activate_Always_SetsTheTokenInTheBody()
        {
            _activationTokens.Activate("token");

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Parameters.Any(p =>
                            p.Type == ParameterType.RequestBody &&
                            (string)p.Value == "token"))));
        }

        [Test]
        public void Activate_Always_SetsTheDeviceTypeInTheBody()
        {
            _activationTokens.Activate("token");

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Parameters.Any(p =>
                p.Type == ParameterType.RequestBody && (string)p.Value == "windows"))));
        }

        [Test]
        public void Activate_IfSuccessful_WillReturnTheActivationData()
        {
            var activationData = new ActivationData();
            var mockResponse = new Mock<IRestResponse<ActivationData>>();
            mockResponse.SetupGet(r => r.Data).Returns(activationData);
            _mockRestClient
                .Setup(rc => rc.Execute<ActivationData>(It.IsAny<IRestRequest>()))
                .Returns(mockResponse.Object);

            var returnedActivationData = _activationTokens.Activate("token");

            Assert.AreSame(activationData, returnedActivationData);
        }

    }
}