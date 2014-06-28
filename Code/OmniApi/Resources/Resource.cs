namespace OmniApi.Resources
{
    using System;
    using System.Configuration;
    using OmniApi.Models;
    using OmniCommon;
    using Refit;

    public abstract class Resource<T>
    {
        #region Fields

        protected readonly T ResourceApi;

        #endregion

        #region Constructors and Destructors

        protected Resource()
        {
            ResourceApi = RestService.For<T>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion

        #region Public Properties

        public Token Token { get; set; }

        public string AccessToken
        {
            get
            {
                return string.Concat("bearer ", Token.access_token);
            }
        }

        #endregion

        public IObservable<TModel> Authorize<TModel>(IObservable<TModel> observable)
        {
            return AuthorizationObserver.Authorize(observable, Token);
        }
    }
}