namespace OmniApi.Resources.v1
{
    using System;
    using Refit;

    public interface IUsersApi
    {
        [Get("/users")]
        IObservable<Models.User> Get([Header("Authorization")] string token);
    }
}