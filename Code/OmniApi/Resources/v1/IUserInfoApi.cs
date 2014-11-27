namespace OmniApi.Resources.v1
{
    using System;
    using Refit;

    public interface IUserInfoApi
    {
        [Get("/user_info")]
        IObservable<Models.UserInfo> Get([Header("Authorization")] string token);
    }
}