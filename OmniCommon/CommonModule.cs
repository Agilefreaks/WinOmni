using Ninject.Modules;
using OmniCommon.Interfaces;
using OmniCommon.Services;

namespace OmniCommon
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniService>().To<OmniService>().InSingletonScope();
        }
    }
}