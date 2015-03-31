namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Dto;
    using Refit;

    public interface IClippingsApi
    {
        #region Public Methods and Operators

        [Get("/clippings/{id}")]
        IObservable<ClippingDto> Get([AliasAs("id")] string id, [Header("Authorization")] string token);

        [Post("/clippings")]
        IObservable<ClippingDto> Create([Body] ClippingDto clippingDto, [Header("Authorization")] string token);

        [Get("/clippings/last")]
        IObservable<ClippingDto> Last([Header("Authorization")] string token);

        #endregion
    }
}