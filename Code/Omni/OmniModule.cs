namespace Omni
{
    using Ninject.Modules;

    public class OmniModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IConnectionManager>().To<ConnectionManager>();
            Kernel.Bind<IOmniService>().To<OmniService>().InSingletonScope();
        }
    }
}