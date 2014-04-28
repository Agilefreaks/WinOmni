using System.Configuration;

namespace OmniApi
{
    using Ninject.Modules;
    using Retrofit.Net;
    using global::OmniApi.Resources;

    public class OmniApiModule : NinjectModule
    {
        private readonly string _baseUrl;

        public OmniApiModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IDevicesAPI>().ToConstant(GetAPIEndpoint<IDevicesAPI>());
            Kernel.Bind<IAuthorizationAPI>().ToConstant(GetAPIEndpoint<IAuthorizationAPI>());
        }

        private T GetAPIEndpoint<T>()
            where T : class
        {
            var restAdapter = new RestAdapter(_baseUrl);
            return restAdapter.Create<T>();
        }
    }
}