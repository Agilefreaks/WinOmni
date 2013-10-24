namespace OmniApiTests.Resources
{
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources;
    using RestSharp;

    [TestFixture]
    public class ActivationTokensTests
    {
        private Mock<IRestClient> _mockRestClient;

        private ActivationTokens _activationTokens;

        [SetUp]
        public void SetUp()
        {
            _mockRestClient = new Mock<IRestClient>();
            _activationTokens = ApiResource.Build<ActivationTokens>("http://test.omnipasteapp.com", _mockRestClient.Object);

            _mockRestClient.Setup(rc => rc.Execute<ActivationModel>(It.IsAny<IRestRequest>()))
                           .Returns(new RestResponse<ActivationModel>());
        }

        [Test]
        public void ResourceKeyAlwaysIsCorrect()
        {
            Assert.AreEqual(this._activationTokens.ResourceKey, "activation_tokens");
        }

        [Test]
        public void ActivateAlwaysMakesaPutRequest()
        {
            this._activationTokens.Activate(string.Empty);

            this._mockRestClient.Verify(rc => rc.Execute<ActivationModel>(It.Is<IRestRequest>(rr => rr.Method == Method.PUT)));
        }

        [Test]
        public void ActivateAlwaysMakesARequestToTheCorrectResource()
        {
            this._activationTokens.Activate(string.Empty);

            this._mockRestClient.Verify(rc => rc.Execute<ActivationModel>(It.Is<IRestRequest>(rr => rr.Resource == "activation_tokens")));
        }

        [Test]
        public void ActivateAlwaysSetsTheCorrectBody()
        {
            this._activationTokens.Activate("token");

            this._mockRestClient.Verify(rc => rc.Execute<ActivationModel>(It.Is<IRestRequest>(rr => rr.Parameters.Any(p =>
                p.Type == ParameterType.RequestBody 
                && p.Name == "application/json" 
                && (string)p.Value == "{\r\n  \"token\": \"token\",\r\n  \"device\": \"windows\"\r\n}"))));
        }

        [Test]
        public void ActivateIfSuccessfulWillReturnTheActivationData()
        {
            var activationData = new ActivationModel();
            var mockResponse = new Mock<IRestResponse<ActivationModel>>();
            mockResponse.SetupGet(r => r.Data).Returns(activationData);
            this._mockRestClient
                .Setup(rc => rc.Execute<ActivationModel>(It.IsAny<IRestRequest>()))
                .Returns(mockResponse.Object);

            var returnedActivationData = this._activationTokens.Activate("token");

            Assert.AreSame(activationData, returnedActivationData);
        }
    }
}