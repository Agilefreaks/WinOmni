namespace Clipboard.API.Resources.v1
{
    using System;
    using System.Net.Http;
    using Clipboard.Dto;
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

        public IObservable<ClippingDto> Last()
        {
            return ResourceApi.Last(AccessToken);
        }

        public IObservable<ClippingDto> Create(string deviceId, string content)
        {
            return ResourceApi.Create(new ClippingDto(content, deviceId), AccessToken);
        }

        public IObservable<ClippingDto> Get(string id)
        {
            return ResourceApi.Get(id, AccessToken);
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