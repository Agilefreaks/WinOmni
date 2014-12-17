namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Models;

    public interface ISyncs
    {
        IObservable<EmptyModel> Post(Sync sync);
    }
}