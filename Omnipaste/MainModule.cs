using Ninject.Modules;
using PubNubClipboard;
using PubNubClipboard.Impl.PubNub;
using PubNubClipboard.Services;
using WindowsClipboard.Interfaces;

namespace Omnipaste
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            var mainForm = new MainForm();
            Kernel.Bind<MainForm>().ToConstant(mainForm);
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToConstant(mainForm);
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
#else
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
#endif
            Kernel.Bind<IPubNubClipboard>().To<PubNubOmniclipboard>().InSingletonScope();
        }
    }
}
