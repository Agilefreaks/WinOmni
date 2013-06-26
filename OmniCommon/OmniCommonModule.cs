using Ninject.Modules;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommon
{
    using OmniCommon.DataProviders;
    using OmniCommon.Services.ActivationServiceData;

    public class OmniCommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniService>().To<OmniService>().InSingletonScope();

            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
            Kernel.Bind<IConfigurationService>().To<ConfigurationService>();
        }
    }
}