namespace OmniApi
{
    using System.Configuration;
    using Ninject.Modules;
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
            Kernel.Bind<IAuthorizationAPI>().ToConstant(GetApiEndpoint<IAuthorizationAPI>());
        }

        private T GetApiEndpoint<T>()
            where T : class
        {
            var restAdapter = new RestAdapter(_baseUrl);
            return restAdapter.Create<T>();
        }
    }
}