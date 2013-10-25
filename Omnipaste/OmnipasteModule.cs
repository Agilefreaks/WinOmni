namespace Omnipaste
{
    using Ninject;
    using Ninject.Modules;
    using OmniCommon.DataProviders;
    using Omnipaste.DataProviders;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;
    using Omnipaste.Shell;
    using WindowsClipboard.Interfaces;

    public class OmnipasteModule : NinjectModule
    {
        public override void Load()
        {
            // activation service dependency injection
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentInfoProvider>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToMethod(c => c.Kernel.Get<IShellViewModel>());

            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
        }
    }
}