namespace Omnipaste
{
    using Ninject.Modules;
    using OmniCommon.DataProviders;
    using Omnipaste.DataProviders;
    using Omnipaste.Services;
    using Omnipaste.Services.ActivationServiceData;

    public class OmnipasteModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentInfoProvider>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();

            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
        }
    }
}