namespace OmniApiTests
{
    using System;
    using System.Net.Http;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Moq;
    using NUnit.Framework;
    using OmniApi;
    using OmniApi.Models;
    using OmniApi.Resources.v1;

    public class AuthorizationObserverTests
    {
        private Mock<IOAuth2> _mockOAuth2;

        private Subject<string> _observable;

        private Mock<IObserver<string>> _mockObserver;

        [SetUp]
        public void Setup()
        {
            _observable = new Subject<string>();
            _mockObserver = new Mock<IObserver<string>>();
            _mockOAuth2 = new Mock<IOAuth2>();
        }

        [Test]
        public void Authorize_Always_CallsOnNext()
        {
            AuthorizationObserver.Authorize(_observable, new Token(), _mockOAuth2.Object).Subscribe(_mockObserver.Object);
            _observable.OnNext("42");

            _mockObserver.Verify(m => m.OnNext("42"), Times.Once);
        }

        [Test]
        public void Authorize_Always_CallsOnComplete()
        {
            AuthorizationObserver.Authorize(_observable, new Token(), _mockOAuth2.Object).Subscribe(_mockObserver.Object);
            _observable.OnCompleted();

            _mockObserver.Verify(m => m.OnCompleted(), Times.Once);
        }

        [Test]
        public void Authorize_WhenErrorIsNotHttpException_WillCallOnError()
        {
            AuthorizationObserver.Authorize(_observable, new Token(), _mockOAuth2.Object).Subscribe(_mockObserver.Object);
            _observable.OnError(new NotImplementedException());

            _mockObserver.Verify(m => m.OnError(It.IsAny<NotImplementedException>()), Times.Once);
        }

        [Test]
        public void Authorize_WhneErrorIsHttpRequestException_WillCallOauth2Refresh()
        {
            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(Observable.Empty<Token>());

            AuthorizationObserver.Authorize(_observable, new Token("access token", "refresh token"), _mockOAuth2.Object).Subscribe(_mockObserver.Object);
            _observable.OnError(new HttpRequestException("Response status code does not indicate success: 401 (Unauthorized)."));

            _mockOAuth2.Verify(m => m.Refresh("refresh token"), Times.Once);
        }
    }
}