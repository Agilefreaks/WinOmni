namespace OmniApi.Resources.v1
{
    using System;

    public interface IUsers
    {
        IObservable<Models.User> Get();
    }
}