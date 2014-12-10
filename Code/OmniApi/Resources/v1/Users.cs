namespace OmniApi.Resources.v1
{
    using System;
    using System.Net.Http;
    using OmniCommon.Interfaces;
    using Refit;

    public class Users : ResourceWithAuthorization<IUsersApi>, IUsers
    {
        public Users(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
            : base(configurationService, webProxyFactory)
        {
        }

        public IObservable<Models.User> Get()
        {
            return ResourceApi.Get(AccessToken);
        }


        protected override IUsersApi CreateResourceApi(HttpClient httpClient)
        {
            return RestService.For<IUsersApi>(httpClient);
        }
    }
}