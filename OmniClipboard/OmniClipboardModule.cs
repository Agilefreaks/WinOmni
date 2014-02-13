namespace OmniClipboard
{
    using Ninject.Modules;
    using global::OmniClipboard.Messaging;

    public class OmniClipboardModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IPubNubClientFactory>().To<PubNubClientFactory>().InSingletonScope();
            Kernel.Bind<IMessagingService>().To<PubNubMessagingService>();
        }
    }
}