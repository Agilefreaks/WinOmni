namespace OmniApi.Resources.v1
{
    using System;

    public interface IUserInfo
    {
        IObservable<Models.UserInfo> Get();
    }
}