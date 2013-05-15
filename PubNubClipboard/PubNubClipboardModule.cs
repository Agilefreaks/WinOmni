using Ninject.Modules;
using PubNubClipboard.Impl.PubNub;

namespace PubNubClipboard
{
    using PubNubClipboard = PubNubClipboard.Impl.PubNub.PubNubClipboard;

    public class PubNubClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPubNubClipboard>().To<PubNubClipboard>().InSingletonScope();
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
        }
    }
}