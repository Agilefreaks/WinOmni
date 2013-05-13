using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;
using PubNubClipboard;
using PubNubClipboard.Services;
using WindowsClipboard.Interfaces;

namespace Omnipaste
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<MainForm>().ToSelf().InSingletonScope();
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToMethod(c => c.Kernel.Get<MainForm>());
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
#endif
            Kernel.Bind<IOmniClipboard>().ToMethod(c => c.Kernel.Get<IPubNubClipboard>());
            Kernel.Bind<ILocalClipboard>().ToMethod(c => c.Kernel.Get<IWindowsClipboard>());
        }
    }
}
