namespace OmniApi
{
    using Ninject.Modules;
    using OmniApi.Resources.v1;

    public class OmniApiModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOAuth2>().To<OAuth2>();
            Kernel.Bind<IDevices>().To<Devices>();
            Kernel.Bind<IUsers>().To<Users>();
        }
    }
}