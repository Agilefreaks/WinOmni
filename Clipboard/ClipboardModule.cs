using System.Configuration;
using WindowsClipboard;
using Ninject;
using Clipboard.Handlers;
using Ninject.Modules;
using OmniCommon.Interfaces;
using Retrofit.Net;

namespace Clipboard
{
    public class ClipboardModule : NinjectModule
    {
        private readonly string _baseUrl;

        public IConfigurationService ConfigurationService { get; set; }

        public ClipboardModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        }

        public override void Load()
        {
            ConfigurationService = Kernel.Get<IConfigurationService>();
        
            Kernel.Bind<IOmniMessageHandler>().To<IncomingClippingsHandler>();
            Kernel.Bind<IClippingsAPI>().ToConstant(GetClippingsAPI());
            Kernel.Bind<IOutgoingClippingHandler>().To<OutgoingClippingsHandler>();
            Kernel.Bind<IStartable>().To<OutgoingClippingsHandler>();
            Kernel.Load(new WindowsClipboardModule());
        }

        public IClippingsAPI GetClippingsAPI()
        {
            var adapter = new RestAdapter(_baseUrl);
            return adapter.Create<IClippingsAPI>();
        }
    }
}