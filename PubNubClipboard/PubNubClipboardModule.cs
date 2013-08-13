using Ninject.Modules;
using OmniCommon.Interfaces;
using PubNubClipboard.Api;
using PubNubClipboard.Messaging;

namespace PubNubClipboard
{
    public class PubNubClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniClipboard>().To<PubNubClipboard>().InSingletonScope();
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IOmniApi>().To<OmniApi>().InSingletonScope();
            Kernel.Bind<IMessagingService>().To<PubNubMessagingService>();
        }
    }
}