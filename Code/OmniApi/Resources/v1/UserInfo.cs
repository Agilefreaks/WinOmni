namespace OmniApi.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniCommon.Interfaces;
    using Refit;

    public class UserInfo : ResourceWithAuthorization<IUserInfoApi>, IUserInfo
    {
        public UserInfo(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        public IObservable<Models.UserInfo> Get()
        {
            return Authorize(ResourceApi.Get(AccessToken));
        }


        protected override IUserInfoApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IUserInfoApi>(httpClient);
        }
    }
}