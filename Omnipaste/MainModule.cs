using Ninject.Modules;
using Omniclipboard;
using Omniclipboard.Impl.PubNub;
using Omniclipboard.Services;

namespace Omnipaste
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<MainForm>().To<MainForm>();
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
            Kernel.Bind<IOmniclipboard>().To<PubNubOmniclipboard>().InSingletonScope();
        }
    }
}
