namespace OmniApi.Support
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading;
    using System.Threading.Tasks;
    using OmniCommon.Helpers;
    using OmniApi.Models;
    using OmniApi.Resources.v1;

    public class HttpClientWithAuthorizationHandler : DelegatingHandler
    {
        #region Fields

        private readonly IOAuth2 _oAuth2;
        private readonly Token _token;
        private readonly IHttpResponseMessageHandler _responseMessageHandler;

        #endregion

        #region Constructors and Destructors
        public HttpClientWithAuthorizationHandler(IOAuth2 oAuth2, Token token, IHttpResponseMessageHandler responseMessageHandler)
        {
            _oAuth2 = oAuth2;
            _token = token;
            _responseMessageHandler = responseMessageHandler;
        }

        #endregion

        #region Public Methods
        
        public IObservable<HttpResponseMessage> Handle(IObservable<HttpResponseMessage> requestObservable)
        {
            return requestObservable
                .Select(message => message.StatusCode == HttpStatusCode.Unauthorized
                    ? RefreshTokenAndRetryRequest(requestObservable)
                    : Observable.Return(message, SchedulerProvider.Default))
                .Switch();
        }

        #endregion

        #region Protected Methods

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestObservable = Observable.Defer(() => base.SendAsync(request, cancellationToken).ToObservable());
            return Handle(requestObservable).ToTask(cancellationToken);
        }

        protected IObservable<HttpResponseMessage> RefreshTokenAndRetryRequest(IObservable<HttpResponseMessage> requestObservable)
        {
            return _oAuth2.Refresh(_token.RefreshToken)
                .Select(_ => requestObservable)
                .Switch()
                .Catch<HttpResponseMessage, Exception>(exception =>
                {
                    var apiException = exception.GetApiException();
                    if (apiException.HasStatusCode(HttpStatusCode.BadRequest))
                    {
                        Observable.Start(_responseMessageHandler.OnBadRequest);
                    }

                    return Observable.Throw<HttpResponseMessage>(exception);
                });
        }

        #endregion
    }
}
