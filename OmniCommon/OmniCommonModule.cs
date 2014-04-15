using Caliburn.Micro;
using Ninject.Modules;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommon
{

    public class OmniCommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            Kernel.Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
        }
    }
}