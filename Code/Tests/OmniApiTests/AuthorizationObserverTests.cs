namespace OmniApiTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Refit;

    public class AuthorizationObserverTests
    {
        private Mock<IOAuth2> _mockOAuth2;

        private Subject<string> _observable;

        private ITestableObserver<string> _testableObserver;

        private Mock<ISessionManager> _mockSessionManager;

        [SetUp]
        public void Setup()
        {
            _observable = new Subject<string>();
            _testableObserver = new TestScheduler().CreateObserver<string>();
            _mockOAuth2 = new Mock<IOAuth2>();
            _mockSessionManager = new Mock<ISessionManager>();
        }

        [Test]
        public void Authorize_Always_CallsOnNext()
        {
            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token())
                .Subscribe(_testableObserver);
            
            _observable.OnNext("42");

            _testableObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnNext);
        }

        [Test]
        public void Authorize_Always_CallsOnComplete()
        {
            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token())
                .Subscribe(_testableObserver);

            _observable.OnCompleted();

            _testableObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnCompleted);
        }

        [Test]
        public void Authorize_WhenErrorIsNotHttpException_WillCallOnError()
        {
            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token())
                .Subscribe(_testableObserver);

            _observable.OnError(new NotImplementedException());

            _testableObserver.Messages.Should().Contain(m => m.Value.Kind == NotificationKind.OnError);
        }

        [Test]
        public void Authorize_WhenErrorIsHttpRequestException_WillCallOAuth2Refresh()
        {
            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(Observable.Empty<Token>());

            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token("access token", "refresh token"))
                .Subscribe(_testableObserver);

            var createUnauthorizedException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            createUnauthorizedException.Wait();

            _observable.OnError(createUnauthorizedException.Result);

            _mockOAuth2.Verify(m => m.Refresh("refresh token"), Times.Once);
        }

        [Test]
        public void Authorize_WhenRefreshThrowsRequestException_WillCallSessionManagerLogout()
        {
            var refreshObservable = new Subject<Token>();
            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(refreshObservable);

            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token("access token", "refresh token"))
                .Subscribe(_testableObserver);

            var createUnauthorizedException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            createUnauthorizedException.Wait();
            var createBadRequestException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.BadRequest));
            createBadRequestException.Wait();

            _observable.OnError(createUnauthorizedException.Result);
            refreshObservable.OnError(createBadRequestException.Result);

            _mockSessionManager.Verify(m => m.LogOut(), Times.Once);
        }

        [Test]
        public void Authorize_WhenRefreshThrowsRequestException_ForwardsTheGivenError()
        {
            var refreshObservable = new Subject<Token>();
            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(refreshObservable);

            AuthorizationObserver
                .Authorize(_observable, _mockOAuth2.Object, _mockSessionManager.Object, new Token("access token", "refresh token"))
                .Subscribe(_testableObserver);

            var createUnauthorizedException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            createUnauthorizedException.Wait();
            var createBadRequestException = ApiException.Create(new HttpResponseMessage(HttpStatusCode.BadRequest));
            createBadRequestException.Wait();

            _observable.OnError(createUnauthorizedException.Result);
            refreshObservable.OnError(createBadRequestException.Result);

            _testableObserver.Messages.Should()
                .Contain(
                    m =>
                    m.Value.Kind == NotificationKind.OnError && m.Value.Exception == createBadRequestException.Result);
        }
    }
}