using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;
using Omnipaste.Services;
using PubNubClipboard;
using WindowsClipboard.Interfaces;

namespace Omnipaste
{
    public class MainModule : NinjectModule
    {
#if DEBUG
        public const string ApplicationName = "Omnipaste-Debug";
#elif STAGING
        public const string ApplicationName = "Omnipaste-Staging";
#else
        public const string ApplicationName = "Omnipaste";
#endif

        public override void Load()
        {
            Kernel.Bind<MainForm>().ToSelf().InSingletonScope();
            Kernel.Bind<ConfigureForm>().ToSelf().InSingletonScope();
            Kernel.Bind<IDelegateClipboardMessageHandling>().ToMethod(c => c.Kernel.Get<MainForm>());
            Kernel.Bind<IConfigurationProvider>().To<DPAPIConfigurationProvider>().InSingletonScope();
#if DEBUG
            Kernel.Bind<IActivationDataProvider>().To<MockActivationDataProvider>().InSingletonScope();
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<MockApplicationDeploymentInfoProvider>();
#else
            Kernel.Bind<IActivationDataProvider>().To<ClickOnceActivationDataProvider>().InSingletonScope();
            Kernel.Bind<IApplicationDeploymentInfoProvider>().To<ApplicationDeploymentWrapper>().InSingletonScope();
#endif
            Kernel.Bind<IOmniClipboard>().ToMethod(c => c.Kernel.Get<IPubNubClipboard>());
            Kernel.Bind<ILocalClipboard>().ToMethod(c => c.Kernel.Get<IWindowsClipboard>());
        }
    }
}
