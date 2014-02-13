namespace Clipboard
{
    using Clipboard.Handlers;
    using Ninject.Modules;
    using OmniCommon.Interfaces;
    using Retrofit.Net;

    public class ClipboardModule : NinjectModule
    {
        private readonly string _baseUrl;

        public ClipboardModule(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public override void Load()
        {
            Kernel.Bind<IOmniMessageHandler>().To<ClippingHandler>();
            Kernel.Bind<IClippingsAPI>().ToConstant(GetClippingsAPI());
        }

        public IClippingsAPI GetClippingsAPI()
        {
            var adapter = new RestAdapter(_baseUrl);
            return adapter.Create<IClippingsAPI>();
        }
    }
}