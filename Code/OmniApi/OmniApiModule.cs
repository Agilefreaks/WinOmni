namespace OmniApi
{
    using System.Configuration;
    using Ninject;
    using Ninject.Modules;
    using OmniApi.Models;
    using OmniApi.Resources.v1;
    using Retrofit.Net;
    using global::OmniApi.Resources;

    public class OmniApiModule : NinjectModule
    {
        private readonly string _baseUrl;

        public OmniApiModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IOAuth2>().To<OAuth2>();
            Kernel.Bind<IDevicesApi>().ToMethod(x => GetApiEndpoint<IDevicesApi, Device>());
        }

        private T GetApiEndpoint<T, TR>()
            where T : class
            where TR : class
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);

            return restAdapter.Create<T, TR>();
        }
    }
}