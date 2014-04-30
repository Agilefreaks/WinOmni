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
        private string _baseUrl;

        public DevicesModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IDevicesAPI>().ToMethod(c => GetAPIEndpoint<IDevicesAPI, Device>());
        }

        private T GetAPIEndpoint<T, R>()
            where T : class
            where R : class
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);
            
            return restAdapter.Create<T, R>();
        }
    }
}