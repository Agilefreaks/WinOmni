namespace Omnipaste.OmniClipboard.Infrastructure.Tests.Api
{
    using System;
    using System.Linq;
    using System.Net;
    using Moq;
    using NUnit.Framework;
    using Omnipaste.OmniClipboard.Core.Api;
    using Omnipaste.OmniClipboard.Core.Api.Models;
    using Omnipaste.OmniClipboard.Infrastructure.Api.Resources;
    using RestSharp;

    [TestFixture]
    public class ClippingsTests
    {
        private Clippings _subject;

        private Mock<IRestClient> _mockRestClient;

        [SetUp]
        public void SetUp()
        {
            _mockRestClient = new Mock<IRestClient>();
            _subject = new Clippings(_mockRestClient.Object);
        }

        [Test]
        public void ApiUrl_Set_WillSetTheBaseUrlOnRestClient()
        {
            _subject.ApiUrl = "http://test.com/v1";

            _mockRestClient.VerifySet(c => c.BaseUrl = "http://test.com/v1");
        }

        [Test]
        public void GetLastAsync_Always_CallsRestClientGetForJsonData()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();

            _subject.GetLastAsync(mockHandler.Object);

            _mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.RequestFormat == DataFormat.Json),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsync_Always_CallsForAClippingResource()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();

            _subject.GetLastAsync(mockHandler.Object);

            _mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.Resource == "clippings"),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsync_Always_AddsTheApiKeyInTheHeader()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();
            _subject.ApiKey = "secret_api_key";

            _subject.GetLastAsync(mockHandler.Object);

            _mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.Parameters.Any(p => p.Name == "Channel" && p.Type == ParameterType.HttpHeader && p.Value == "secret_api_key")),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsync_OnSuccess_CallsHandlerToHandleTheResponse()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();
            _subject.ApiKey = "secret_api_key";
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
            _mockRestClient.Setup(c => c.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
                    {
                        responseMock.Setup(r => r.Data).Returns(new Clipping { Content="content", Token = "secret_api_key" });
                        
                        callback(responseMock.Object, null);
                    });

            _subject.GetLastAsync(mockHandler.Object);

            mockHandler.Verify(h => h.HandleClipping(It.Is<string>(s => s == responseMock.Object.Data.Content)));
        }

        [Test]
        public void SaveAsync_Always_PostsToTheCorrectUrl()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            
            _subject.SaveAsync("data", mockHandler.Object);

            _mockRestClient.Verify(rc => 
                rc.ExecuteAsync(
                    It.Is<IRestRequest>(r => 
                        r.Method == Method.POST && 
                        r.Parameters.Any(p => 
                            p.Type == ParameterType.RequestBody &&
                            (string)p.Value == "{\r\n  \"token\": null,\r\n  \"content\": \"data\"\r\n}")),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void SaveAsync_WhenPostIsCompleteAndSuccessful_WillCallTheHandlerSaveClippingSucceeded()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.Created);
            _mockRestClient.Setup(rc => rc.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
            .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
            {
                responseMock.Setup(r => r.Data).Returns(new Clipping { Content = "content", Token = "secret_api_key" });

                callback(responseMock.Object, null);
            });

            _subject.SaveAsync("data", mockHandler.Object);

            mockHandler.Verify(h => h.SaveClippingSucceeded());
        }

        [Test]
        public void SaveAsync_WhenPostIsCompleteAndNotSuccessful_WillCallTheHandlerSaveClippingFailed()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.BadRequest);
            _mockRestClient.Setup(rc => rc.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
            .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
            {
                responseMock.Setup(r => r.Data).Returns(new Clipping { Content = "content", Token = "secret_api_key" });

                callback(responseMock.Object, null);
            });

            _subject.SaveAsync("data", mockHandler.Object);

            mockHandler.Verify(h => h.SaveClippingFailed(responseMock.Object.Content));
        }
    }
}
