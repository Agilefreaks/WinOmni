namespace OmnipasteWPF.ViewModels
{
    using Caliburn.Micro;
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
            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IApplicationWrapper>().To<ApplicationWrapper>().InSingletonScope();
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<OnlineActivationDataProvider>().InSingletonScope();
#endif
        }
    }
}