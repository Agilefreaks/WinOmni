namespace OmnipasteWPF.ViewModels.MainView
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
            this.Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
            this.Kernel.Bind<IStepFactory>().To<StepFactory>();
            this.Kernel.Bind<IDependencyResolver>().ToConstant(this);
            this.Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentWrapper>();
            this.Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>();
            this.Kernel.Bind<IConfigurationService>().To<ConfigurationService>();
            this.Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            this.Kernel.Bind<IApplicationWrapper>().To<ApplicationWrapper>().InSingletonScope();
#if DEBUG
            this.Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<OnlineActivationDataProvider>().InSingletonScope();
#endif
        }
    }
}