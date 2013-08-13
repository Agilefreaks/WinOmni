namespace Omnipaste
{
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.DataProviders;
    using Omnipaste.DataProviders;
    using Omnipaste.Shell;
    using WindowsClipboard.Interfaces;

    public class OmnipasteModule : NinjectModule
    {
        public override void Load()
        {
            // activation service dependency injection
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentInfoProvider>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();
            Kernel.Bind<IAppConfigurationProvider>().To<AppConfigurationProvider>();
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToMethod(c => c.Kernel.Get<IShellViewModel>());
#if DEBUG
            this.Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<OnlineActivationDataProvider>().InSingletonScope();
#endif
        }
    }
}