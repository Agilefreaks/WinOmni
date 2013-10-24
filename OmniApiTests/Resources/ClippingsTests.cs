namespace OmniApiTests.Resources
{
    using System;
    using System.Linq;
    using System.Net;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Models;
    using OmniApi.Resources;
    using Omnipaste.OmniClipboard.Core.Api;
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
            _subject = ApiResource.Build<Clippings>("http://test.omnipasteapp.com", this._mockRestClient.Object);
        }

        [Test]
        public void GetLastAsyncAlwaysCallsRestClientGetForJsonData()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();

            this._subject.GetLastAsync(mockHandler.Object);

            this._mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.RequestFormat == DataFormat.Json),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsyncAlwaysCallsForAClippingResource()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();

            this._subject.GetLastAsync(mockHandler.Object);

            this._mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.Resource == "clippings"),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsyncAlwaysAddsTheApiKeyInTheHeader()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();
            this._subject.ApiKey = "secret_api_key";

            this._subject.GetLastAsync(mockHandler.Object);

            this._mockRestClient.Verify(
                c =>
                c.ExecuteAsync(
                    It.Is<IRestRequest>(r => r.Parameters.Any(p => p.Name == "Channel" && p.Type == ParameterType.HttpHeader && p.Value == "secret_api_key")),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void GetLastAsyncOnSuccessCallsHandlerToHandleTheResponse()
        {
            var mockHandler = new Mock<IGetClippingCompleteHandler>();
            this._subject.ApiKey = "secret_api_key";
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
            this._mockRestClient.Setup(c => c.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
                .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
                    {
                        responseMock.Setup(r => r.Data)
                            .Returns(new Clipping { Content = "content", Token = "secret_api_key" });

                        callback(responseMock.Object, null);
                    });

            this._subject.GetLastAsync(mockHandler.Object);

            mockHandler.Verify(h => h.HandleClipping(It.Is<string>(s => s == responseMock.Object.Data.Content)));
        }

        [Test]
        public void SaveAsyncAlwaysPostsToTheCorrectUrl()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            
            this._subject.SaveAsync("data", mockHandler.Object);

            this._mockRestClient.Verify(rc => 
                rc.ExecuteAsync(
                    It.Is<IRestRequest>(r => 
                        r.Method == Method.POST && 
                        r.Parameters.Any(p => 
                            p.Type == ParameterType.RequestBody &&
                            (string)p.Value == "{\r\n  \"token\": null,\r\n  \"content\": \"data\"\r\n}")),
                    It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()));
        }

        [Test]
        public void SaveAsyncWhenPostIsCompleteAndSuccessfulWillCallTheHandlerSaveClippingSucceeded()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.Created);
            this._mockRestClient.Setup(rc => rc.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
            .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
            {
                responseMock.Setup(r => r.Data).Returns(new Clipping { Content = "content", Token = "secret_api_key" });

                callback(responseMock.Object, null);
            });

            this._subject.SaveAsync("data", mockHandler.Object);

            mockHandler.Verify(h => h.SaveClippingSucceeded());
        }

        [Test]
        public void SaveAsyncWhenPostIsCompleteAndNotSuccessfulWillCallTheHandlerSaveClippingFailed()
        {
            var mockHandler = new Mock<ISaveClippingCompleteHandler>();
            var responseMock = new Mock<IRestResponse<Clipping>>();
            responseMock.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.BadRequest);
            this._mockRestClient.Setup(rc => rc.ExecuteAsync(
                It.IsAny<IRestRequest>(), 
                It.IsAny<Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>()))
            .Callback<IRestRequest, Action<IRestResponse<Clipping>, RestRequestAsyncHandle>>((request, callback) =>
            {
                responseMock.Setup(r => r.Data).Returns(new Clipping { Content = "content", Token = "secret_api_key" });

                callback(responseMock.Object, null);
            });

            this._subject.SaveAsync("data", mockHandler.Object);

            mockHandler.Verify(h => h.SaveClippingFailed(responseMock.Object.Content));
        }
    }
}
