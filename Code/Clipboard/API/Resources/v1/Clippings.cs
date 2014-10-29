namespace Clipboard.API.Resources.v1
{
    using System;
    using System.Configuration;
    using Clipboard.Models;
    using OmniApi.Resources;
    using OmniCommon;
    using Refit;

    public class Clippings : Resource<IClippingsApi>, IClippings
    {
        #region Interfaces

        #endregion

        #region Constructors and Destructors

        public Clippings()
            : base(CreateResourceApi())
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

        private static IClippingsApi CreateResourceApi()
        {
            return RestService.For<IClippingsApi>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion

    }
}