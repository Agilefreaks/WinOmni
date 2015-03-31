namespace OmniApi.Resources.v1
{
    using System;
    using OmniApi.Dto;

    public interface IUsers
    {
        IObservable<UserDto> Get();
    }
}