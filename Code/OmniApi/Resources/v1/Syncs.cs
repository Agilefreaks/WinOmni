namespace OmniApi.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniApi.Models;
    using OmniCommon.Interfaces;
    using Refit;

    public class Syncs : ResourceWithAuthorization<ISyncApi>, ISyncs
    {
        #region Constructors and Destructors

        public Syncs(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<EmptyModel> Post(Sync sync)
        {
            return ResourceApi.Post(sync, AccessToken);
        }

        #endregion

        #region Methods

        protected override ISyncApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<ISyncApi>(httpClient);
        }

        #endregion
    }
}