namespace Clipboard.API.Resources.v1
{
    using System;
    using Clipboard.Models;
    using Refit;

    public interface IClippingsApi
    {
        #region Public Methods and Operators

        [Get("/clippings/{id}")]
        IObservable<Clipping> Get([AliasAs("id")] string id, [Header("Authorization")] string token);

        [Post("/clippings")]
        IObservable<Clipping> Create([Body] Clipping clipping, [Header("Authorization")] string token);

        [Get("/clippings/last")]
        IObservable<Clipping> Last([Header("Authorization")] string token);

        #endregion
    }
}