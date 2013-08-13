using Ninject.Modules;
using OmniCommon.Interfaces;
using Omnipaste.OmniClipboard.Core.Api;
using Omnipaste.OmniClipboard.Core.Messaging;
using Omnipaste.OmniClipboard.Infrastructure.Api;
using Omnipaste.OmniClipboard.Infrastructure.Messaging;

namespace Omnipaste.OmniClipboard.Infrastructure
{
    public class PubNubClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniClipboard>().To<Core.OmniClipboard>().InSingletonScope();
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IOmniApi>().To<OmniApi>().InSingletonScope();
            Kernel.Bind<IMessagingService>().To<PubNubMessagingService>();
        }
    }
}