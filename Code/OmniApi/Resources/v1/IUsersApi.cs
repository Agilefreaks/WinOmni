namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Dto;
    using Refit;

    public interface IUsersApi
    {
        [Get("/user")]
        IObservable<UserDto> Get([Header("Authorization")] string token);
    }
}