namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Models;
    using OmniApi.Resources;
    using Refit;

    public class Clippings : Resource<Clippings.IClippingsApi>, IClippings
    {
        #region Interfaces

        [ColdObservable]
        public interface IClippingsApi
        {
            #region Public Methods and Operators

            [Post("/clippings")]
            IObservable<Clipping> Create([Body] Clipping clipping, [Header("Authorization")] string token);

            [Get("/clippings/last")]
            IObservable<Clipping> Last([Header("Authorization")] string token);

            #endregion
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
    }
}