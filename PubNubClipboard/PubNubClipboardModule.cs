using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;
using PubNubClipboard.Impl.PubNub;

namespace PubNubClipboard
{
    using PubNubClipboard = Impl.PubNub.PubNubClipboard;

    public class PubNubClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPubNubClipboard>().To<PubNubClipboard>().InSingletonScope();
            Kernel.Bind<IOmniClipboard>().ToMethod(context => Kernel.Get<IPubNubClipboard>());
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
        }
    }
}