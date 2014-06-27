namespace Clipboard
{
    using Clipboard.API;
    using System.Configuration;
    using Clipboard.Handlers.WindowsClipboard;
    using Clipboard.Models;
    using Ninject;
    using Clipboard.Handlers;
    using Ninject.Modules;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Retrofit.Net;

    public class ClipboardModule : NinjectModule
    {
        private readonly string _baseUrl;

        public IConfigurationService ConfigurationService { get; set; }

        public ClipboardModule()
        {
            _baseUrl = ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl];
        }

        public override void Load()
        {
            ConfigurationService = Kernel.Get<IConfigurationService>();

            Kernel.Bind<IWindowsClipboardWrapper>().To<WindowsClipboardWrapper>().InSingletonScope();
            Kernel.Bind<ILocalClipboardHandler>().To<LocalClipboardsHandler>().InSingletonScope();
            Kernel.Bind<IOmniClipboardHandler>().To<OmniClipboardHandler>().InSingletonScope();
            Kernel.Bind<IHandler, IClipboadHandler>().To<ClipboardHandler>().InSingletonScope();

            Kernel.Bind<IClippingsApi>().ToMethod(c => GetClippingsApi());
        }

        private IClippingsApi GetClippingsApi()
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);

            return restAdapter.Create<IClippingsApi, Clipping>();
        }
    }
}