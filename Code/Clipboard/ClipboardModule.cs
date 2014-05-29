﻿using System.Configuration;
using WindowsClipboard;
using Clipboard.Models;
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
            _baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        }

        public override void Load()
        {
            ConfigurationService = Kernel.Get<IConfigurationService>();
        
            Kernel.Bind<IOmniMessageHandler>().To<IncomingClippingsHandler>().InSingletonScope();
            Kernel.Bind<IClippingsAPI>().ToMethod(c => this.GetClippingsApi());
            Kernel.Bind<IOutgoingClippingHandler>().To<OutgoingClippingsHandler>();
            Kernel.Bind<IStartable>().To<OutgoingClippingsHandler>();
            Kernel.Load(new WindowsClipboardModule());
        }

        private IClippingsAPI GetClippingsApi()
        {
            var authenticator = Kernel.Get<Authenticator>();
            var restAdapter = new RestAdapter(_baseUrl, authenticator);

            return restAdapter.Create<IClippingsAPI, Clipping>();
        }
    }
}