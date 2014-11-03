namespace OmniApi.Resources
{
    using System;
    using System.Diagnostics;
    using BugFreak;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using OmniCommon.Interfaces;

    public abstract class ResourceWithAuthorization<T> : Resource<T>
    {
        #region Constructors and Destructors

        protected ResourceWithAuthorization(IWebProxyFactory webProxyFactory)
            : base(webProxyFactory)
        {
        }

        #endregion

        #region Public Properties

        public string AccessToken
        {
            get
            {
                if (string.IsNullOrEmpty(Token.AccessToken))
                {
                    var callingMethodName = new StackFrame(1).GetMethod().Name;
                    ReportingService.Instance.BeginReport(
                        new Exception(string.Format("AccessToken is empty when calling {0}", callingMethodName)));
                }
                return string.Concat("bearer ", Token.AccessToken);
            }
        }

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public IOAuth2 OAuth2 { get; set; }

        [Inject]
        public ISessionManager SessionManager { get; set; }

        public Token Token
        {
            get
            {
                return new Token(ConfigurationService.AccessToken, ConfigurationService.RefreshToken);
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<TModel> Authorize<TModel>(IObservable<TModel> observable)
        {
            return AuthorizationObserver.Authorize(observable, OAuth2, SessionManager, Token);
        }

        #endregion
    }
}