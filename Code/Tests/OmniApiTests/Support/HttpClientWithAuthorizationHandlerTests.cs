namespace OmniApiTests.Support
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using OmniApi.Dto;
    using OmniApi.Resources.v1;
    using OmniApi.Support;
    using Refit;

    [TestFixture]
    public class HttpClientWithAuthorizationHandlerTests
    {
        private HttpClientWithAuthorizationHandler _subject;
        private TestScheduler _scheduler;
        private TokenDto _tokenDto;
        private Mock<IOAuth2> _mockOAuth2;
        private Mock<IHttpResponseMessageHandler> _mockResponseMessageHandler;
        private Mock<HttpMessageHandler> _mockInnerHandler;

        [SetUp]
        public void SetUp()
        {
            _scheduler = new TestScheduler();
            _tokenDto = new TokenDto();
            _mockOAuth2 = new Mock<IOAuth2> { DefaultValue = DefaultValue.Mock };
            _mockResponseMessageHandler = new Mock<IHttpResponseMessageHandler>();
            _mockInnerHandler = new Mock<HttpMessageHandler>();
            _subject = new HttpClientWithAuthorizationHandler(_mockOAuth2.Object, _tokenDto, _mockResponseMessageHandler.Object)
            {
                InnerHandler = _mockInnerHandler.Object
            };
        }

        [Test]
        public void Handle_WhenUnauthorizedRequest_CallsOAuth2RefreshToken()
        {
            var observable = _scheduler.CreateColdObservable(
                new Recorded<Notification<HttpResponseMessage>>(100,
                    Notification.CreateOnNext(new HttpResponseMessage(HttpStatusCode.Unauthorized))),
                new Recorded<Notification<HttpResponseMessage>>(200,
                    Notification.CreateOnCompleted<HttpResponseMessage>()));

            _scheduler.Start(() => _subject.Handle(observable));

            _mockOAuth2.Verify(x => x.Refresh(It.IsAny<string>()));
        }

        [Test]
        public void Hanle_WhenUnauthorizedRequestAndFailedOAuth2Refresh_DoesNotRetryRequest()
        {
            var callCount = 0;
            var resourceRequestObservable = Observable.Create<HttpResponseMessage>(observer =>
            {
                observer.OnNext(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                observer.OnCompleted();
                callCount++;

                return Disposable.Empty;
            });
            var oAuth2RefreshObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<TokenDto>>(100,
                    Notification.CreateOnError<TokenDto>(new Exception())));
            
            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(oAuth2RefreshObservable);

            _scheduler.Start(() => _subject.Handle(resourceRequestObservable));

            callCount.Should().Be(1);
        }

        [Test]
        public void Handle_WhenUnauthorizedRequestAndFailedOAuth2Refresh_CallsHandlerOnBadRequest()
        {
            var resourceRequestObservable = Observable.Create<HttpResponseMessage>(observer =>
            {
                observer.OnNext(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                observer.OnCompleted();

                return Disposable.Empty;
            });
            var badRequestException = CreateApiRequestException(HttpStatusCode.BadRequest);
            var oAuth2RefreshObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<TokenDto>>(100, Notification.CreateOnError<TokenDto>(badRequestException)));

            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(oAuth2RefreshObservable);

            _scheduler.Start(() => _subject.Handle(resourceRequestObservable));

            _mockResponseMessageHandler.Verify(m => m.OnBadRequest());
        }

        [Test]
        public void Hanle_WhenUnauthorizedRequestAndFailedOAuth2Refresh_ForwardsException()
        {
            var resourceRequestObservable = Observable.Create<HttpResponseMessage>(observer =>
            {
                observer.OnNext(new HttpResponseMessage(HttpStatusCode.Unauthorized));
                observer.OnCompleted();

                return Disposable.Empty;
            });
            var badRequestException = CreateApiRequestException(HttpStatusCode.BadRequest);
            var oAuth2RefreshObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<TokenDto>>(100,
                    Notification.CreateOnError<TokenDto>(badRequestException)));

            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(oAuth2RefreshObservable);

            _scheduler.Start(() => _subject.Handle(resourceRequestObservable)).Messages.Should()
                .Contain(
                    m =>
                    m.Value.Kind == NotificationKind.OnError && m.Value.Exception == badRequestException);
        }

        [Test]
        public void Handle_WhenUnauthorizedRequestAndSuccessfulOAuth2Refresh_RetriesRequest()
        {
            var callCount = 0;
            var resourceRequestObservable = Observable.Create<HttpResponseMessage>(observer =>
            {
                observer.OnNext(new HttpResponseMessage(callCount == 0 ? HttpStatusCode.Unauthorized : HttpStatusCode.OK));
                observer.OnCompleted();
                callCount++;

                return Disposable.Empty;
            });

            var oAuth2RefreshObservable = _scheduler.CreateColdObservable(
                new Recorded<Notification<TokenDto>>(100,
                    Notification.CreateOnNext(new TokenDto())),
                new Recorded<Notification<TokenDto>>(200,
                    Notification.CreateOnCompleted<TokenDto>()));

            _mockOAuth2.Setup(m => m.Refresh(It.IsAny<string>())).Returns(oAuth2RefreshObservable);

            _scheduler.Start(() => _subject.Handle(resourceRequestObservable));

            callCount.Should().Be(2);
        }

        private static ApiException CreateApiRequestException(HttpStatusCode statusCode)
        {
            var createBadRequestException = ApiException.Create(
                new Uri("http://someUri.com"),
                HttpMethod.Get,
                new HttpResponseMessage(statusCode));
            createBadRequestException.Wait();
            return createBadRequestException.Result;
        }
    }
}
