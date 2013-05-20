using Ninject;
using Ninject.Modules;
using OmniCommon.Interfaces;

namespace PubNubClipboard
{
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