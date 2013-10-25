namespace OmniClipboard
{
    using Ninject.Modules;
    using global::OmniClipboard.Messaging;
    using OmniCommon.Interfaces;

    public class OmniClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IOmniClipboard>().To<OmniClipboard>().InSingletonScope();
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IMessagingService>().To<PubNubMessagingService>();
        }
    }
}