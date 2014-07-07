namespace OmniApi
{
    using System;
    using System.Net;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;
    using Refit;

    public class AuthorizationObserver
    {
        #region Fields

        private readonly IOAuth2 _oAuth2;

        private readonly ISessionManager _sessionManager;

        private readonly Token _token;

        #endregion

        #region Constructors and Destructors

        private AuthorizationObserver(IOAuth2 oAuth2,  ISessionManager sessionManager, Token token)
        {
            _oAuth2 = oAuth2;
            _sessionManager = sessionManager;
            _token = token;
        }

        #endregion

        #region Public Methods and Operators

        public static IObservable<T> Authorize<T>(IObservable<T> observable, ISessionManager sessionManager, Token token)
        {
            return new AuthorizationObserver(new OAuth2(), sessionManager, token).Authorize(observable);
        }

        public static IObservable<T> Authorize<T>(IObservable<T> observable, IOAuth2 oAuth2, ISessionManager sessionManager, Token token)
        {
            return new AuthorizationObserver(oAuth2, sessionManager, token).Authorize(observable);
        }

        #endregion

        #region Methods

        private IObservable<T> Authorize<T>(IObservable<T> observable)
        {
            return Observable.Create<T>(
                observer =>
                    {
                        observable.Subscribe(
                            // OnNext
                            observer.OnNext,
                            // OnError
                            e =>
                                {
                                    if (IsUnauthorized(e))
                                    {
                                        _oAuth2.Refresh(_token.RrefreshToken)
                                            .Concat((IObservable<object>)observable)
                                            .Where(o => o is T)
                                            .Cast<T>()
                                            .Catch<T, Exception>(
                                                error =>
                                                    {
                                                        if (IsBadRequest(error))
                                                        {
                                                            _sessionManager.LogOut();
                                                        }

                                                        return Observable.Empty<T>();
                                                    })
                                            .Subscribe(observer);
                                    }
                                    else
                                    {
                                        observer.OnError(e);
                                    }
                                },
                            // OnComplete
                            observer.OnCompleted);

                        return Disposable.Create(() => { });
                    });
        }

        private ApiException GetApiException(Exception exception)
        {
            ApiException result = null;

            if (exception is ApiException)
            {
                result = exception as ApiException;
            }
            else if (exception is AggregateException && exception.InnerException is ApiException)
            {
                result = exception.InnerException as ApiException;
            }

            return result;
        }

        private bool IsUnauthorized(Exception exception)
        {
            var apiException = GetApiException(exception);

            return apiException != null && apiException.StatusCode == HttpStatusCode.Unauthorized;
        }

        private bool IsBadRequest(Exception exception)
        {
            var apiException = GetApiException(exception);

            return apiException != null && apiException.StatusCode == HttpStatusCode.BadRequest;
        }

        #endregion
    }
}