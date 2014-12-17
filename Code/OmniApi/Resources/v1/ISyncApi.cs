namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;
    using Refit;

    public interface ISyncApi
    {
        [Post("/sync")]
        IObservable<EmptyModel> Post([Body] Sync syncs, [Header("Authorization")] string token);
    }
}
