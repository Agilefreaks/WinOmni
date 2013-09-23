namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Api
{
    using System;
    using System.Linq;
    using Moq;
    using NUnit.Framework;
    using OmniCommon.DataProviders;
    using Omnipaste.OmniClipboard.Infrastructure.Api.Resources;
    using RestSharp;

    [TestFixture]
    public class UsersTests
    {
        private Mock<IRestClient> _mockRestClient;

        private Users _users;

        [SetUp]
        public void SetUp()
        {
            _mockRestClient = new Mock<IRestClient>();
            
            _users = new Users(_mockRestClient.Object);

            _mockRestClient.Setup(rc => rc.Execute<ActivationData>(It.IsAny<IRestRequest>()))
                           .Returns(new RestResponse<ActivationData>());
        }

        [Test]
        public void Users_Always_HasARestClient()
        {
            Assert.AreSame(_users.RestClient, _mockRestClient.Object);
        }

        [Test]
        public void ApiUrl_Set_SetsTheBaseUrlOnTheRestClient()
        {
            _users.ApiUrl = "api url";

            _mockRestClient.VerifySet(rc => rc.BaseUrl = "api url");
        }

        [Test]
        public void ResourceKey_Always_IsCorrect()
        {
            Assert.AreEqual(_users.ResourceKey, "users");
        }

        [Test]
        public void Activate_Always_MakesaGETRequest()
        {
            _users.Activate(string.Empty);

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Method == Method.GET)));
        }

        [Test]
        public void Activate_Always_MakesARequestToTheCorrectResource()
        {
            _users.Activate(string.Empty);

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Resource == _users.ResourceKey)));
        }

        [Test]
        public void Activate_Always_SetsTheTokenInTheBody()
        {
            _users.Activate("token");

            _mockRestClient.Verify(rc => rc.Execute<ActivationData>(It.Is<IRestRequest>(rr => rr.Parameters.Any(p =>
                            p.Type == ParameterType.HttpHeader &&
                            (string)p.Value == "token"))));
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

            var returnedActivationData = _users.Activate("token");

            Assert.AreSame(activationData, returnedActivationData);
        }
    }
}