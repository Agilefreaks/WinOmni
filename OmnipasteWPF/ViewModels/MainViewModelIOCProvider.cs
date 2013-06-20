﻿namespace OmnipasteWPF.ViewModels
{
    using OmniCommon.DataProviders;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using OmniCommon.Services.ActivationServiceData;
    using OmnipasteWPF.DataProviders;
    using OmnipasteWPF.Services;

    public class MainViewModelIOCProvider : NinjectIOCProvider
    {
        public override void SetupContainer()
        {
            base.SetupContainer();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
            Kernel.Bind<IStepFactory>().To<StepFactory>();
            Kernel.Bind<IDependencyResolver>().ToConstant(this);
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentWrapper>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();
            Kernel.Bind<IConfigurationService>().To<ConfigurationService>();
        }
    }
}