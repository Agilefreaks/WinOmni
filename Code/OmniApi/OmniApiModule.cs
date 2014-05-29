using System;
using System.Collections.Generic;
using System.Configuration;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Extensions.Conventions.BindingGenerators;
using Ninject.Modules;
using Ninject.Syntax;
using Retrofit.Net;
using global::OmniApi.Resources;

namespace OmniApi
{
    public class OmniApiModule : NinjectModule
    {
        private readonly string _baseUrl;

        public OmniApiModule()
        {
            _baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        }

        public override void Load()
        {
            Kernel.Bind<IAuthorizationAPI>().ToConstant(GetAPIEndpoint<IAuthorizationAPI>());
        }

        private T GetAPIEndpoint<T>()
            where T : class
        {
            Kernel.Get<Authenticator>();
            
            var restAdapter = new RestAdapter(_baseUrl);
            return restAdapter.Create<T>();
        }
    }
}