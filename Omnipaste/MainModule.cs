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
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
#endif
            Kernel.Bind<IOmniclipboard>().To<PubNubOmniclipboard>().InSingletonScope();
        }
    }
}
