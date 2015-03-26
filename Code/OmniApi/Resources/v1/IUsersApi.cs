namespace OmniApi.Resources.v1
{
    using System;
    using Refit;

    public interface IUsersApi
    {
        [Get("/user")]
        IObservable<Models.User> Get([Header("Authorization")] string token);
    }
}