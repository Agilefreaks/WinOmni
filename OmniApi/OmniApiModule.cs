namespace OmniApi
{
    using Ninject.Modules;
    using Retrofit.Net;
    using global::OmniApi.Resources;

    public class OmniApiModule : NinjectModule
    {
        private readonly string _baseUrl;

        public OmniApiModule(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public override void Load()
        {
            Kernel.Bind<IDevicesAPI>().ToConstant(GetAPIEndpoint<IDevicesAPI>());
            Kernel.Bind<IActivationTokenAPI>().ToConstant(GetAPIEndpoint<IActivationTokenAPI>());
        }

        private T GetAPIEndpoint<T>()
            where T : class
        {
            var restAdapter = new RestAdapter(_baseUrl);
            return restAdapter.Create<T>();
        }
    }
}