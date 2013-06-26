namespace Omnipaste
{
    using Ninject.Modules;
    using OmniCommon.DataProviders;
    using Omnipaste.DataProviders;
    using Omnipaste.Shell;

    public class OmnipasteModule : NinjectModule
    {
        public override void Load()
        {
            // activation service dependency injection
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentInfoProvider>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();

#if DEBUG
            this.Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<OnlineActivationDataProvider>().InSingletonScope();
#endif
        }
    }
}