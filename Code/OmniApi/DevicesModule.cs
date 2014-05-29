using System.Configuration;
using Ninject;
using Ninject.Modules;
using OmniApi.Models;
using OmniApi.Resources;
using Retrofit.Net;

namespace OmniApi
{
    public class DevicesModule : NinjectModule
    {
        private readonly string _baseUrl;

        public DevicesModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IDevicesAPI>().ToMethod(c => this.GetApiEndpoint<IDevicesAPI, Device>());
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