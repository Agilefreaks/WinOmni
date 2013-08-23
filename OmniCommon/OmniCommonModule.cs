namespace OmniCommon
{
    using Ninject.Modules;
    using OmniCommon.Interfaces;
    using OmniCommon.Services;
    using OmniCommon.Services.ActivationServiceData;

    public class OmniCommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniService>().To<OmniService>().InSingletonScope();

            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
            Kernel.Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
            Kernel.Bind<IDateTimeService>().To<DateTimeService>().InSingletonScope();
            Kernel.Bind<IClippingRepository>().To<XmlClippingRepository>().InSingletonScope();
            Kernel.Bind<IFileService>().To<FileService>().InSingletonScope();
            Kernel.Bind<IXmlSerializer>().To<SystemXmlSerializer>().InSingletonScope();
        }
    }
}