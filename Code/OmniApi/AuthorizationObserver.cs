namespace OmniApi
{
    using System;
    using System.Net.Http;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using OmniApi.Models;
    using OmniApi.Resources.v1;

    public class AuthorizationObserver
    {
        #region Fields

        private readonly IOAuth2 _oAuth2;

        private readonly Token _token;

        #endregion

        #region Constructors and Destructors

        private AuthorizationObserver(IOAuth2 oAuth2, Token token)
        {
            _oAuth2 = oAuth2;
            _token = token;
        }

        #endregion

        #region Public Methods and Operators

        public static IObservable<T> Authorize<T>(IObservable<T> observable, Token token)
        {
            return new AuthorizationObserver(new OAuth2(), token).Authorize(observable);
        }

        public static IObservable<T> Authorize<T>(IObservable<T> observable, Token token, IOAuth2 oAuth2)
        {
            return new AuthorizationObserver(oAuth2, token).Authorize(observable);
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

        private HttpRequestException GetHttpRequestExceptions(Exception exception)
        {
            HttpRequestException result = null;

            if (exception is HttpRequestException)
            {
                result = exception as HttpRequestException;
            }
            else if (exception is AggregateException && exception.InnerException is HttpRequestException)
            {
                result = exception.InnerException as HttpRequestException;
            }

            return result;
        }

        private bool IsUnauthorized(Exception exception)
        {
            exception = GetHttpRequestExceptions(exception);

            //  TODO: dumb, but we need the HttpResponseMessage to get the code http://stackoverflow.com/questions/22217619/how-do-i-get-statuscode-from-httprequestexception
            return exception != null && exception.Message == "Response status code does not indicate success: 401 (Unauthorized).";
        }

        #endregion
    }
}