namespace Clipboard.API.Resources.v1
{
    using System;
    using System.Net.Http;
    using Clipboard.Models;
    using OmniApi.Resources;
    using OmniCommon.Interfaces;
    using Refit;

    public class Clippings : ResourceWithAuthorization<IClippingsApi>, IClippings
    {
        #region Interfaces

        #endregion

        #region Constructors and Destructors

        public Clippings(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<Clipping> Last()
        {
            return Authorize(ResourceApi.Last(AccessToken));
        }

        public IObservable<Clipping> Create(string identifier, string content)
        {
            return Authorize(ResourceApi.Create(new Clipping(content, identifier), AccessToken));
        }

        #endregion

        #region Methods

        protected override IClippingsApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IClippingsApi>(httpClient);
        }

        #endregion

    }
}