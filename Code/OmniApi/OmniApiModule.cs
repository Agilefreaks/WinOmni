using System.Configuration;
using Ninject;
using Ninject.Modules;
using Retrofit.Net;
using global::OmniApi.Resources;

namespace OmniApi
{
    public class OmniApiModule : NinjectModule
    {
        private readonly string _baseUrl;

        public OmniApiModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IAuthorizationAPI>().ToConstant(this.GetApiEndpoint<IAuthorizationAPI>());
        }

        private T GetApiEndpoint<T>()
            where T : class
        {
            Kernel.Get<Authenticator>();
            
            var restAdapter = new RestAdapter(_baseUrl);
            return restAdapter.Create<T>();
        }
    }
}