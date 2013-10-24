using Ninject.Modules;
using OmniCommon.Interfaces;
using Omnipaste.OmniClipboard.Core.Messaging;
using Omnipaste.OmniClipboard.Infrastructure.Messaging;
using RestSharp;

namespace Omnipaste.OmniClipboard.Infrastructure
{
    using Omnipaste.OmniClipboard.Infrastructure.Services;
    using Omnipaste.OmniClipboard.Infrastructure.Services.ActivationServiceData;

    public class OmniClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IStepFactory>().To<StepFactory>().InSingletonScope();
            Kernel.Bind<IActivationService>().To<ActivationService>().InSingletonScope();
            Kernel.Bind<IConfigurationManager>().To<ConfigurationManager>().InSingletonScope();

            Kernel.Bind<IOmniClipboard>().To<Core.OmniClipboard>().InSingletonScope();
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IMessagingService>().To<PubNubMessagingService>();
        }
    }
}