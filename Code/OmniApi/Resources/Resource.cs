﻿namespace OmniApi.Resources
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    using OmniApi.Support.Converters;
    using OmniApi.Support.Serialization;
    using OmniCommon;
    using OmniCommon.Interfaces;

    public abstract class Resource<T> : IProxyConfigurationObserver
    {
        protected readonly IConfigurationService ConfigurationService;

        #region Constructors and Destructors

        protected Resource(IConfigurationService configurationService, IWebProxyFactory webProxyFactory)
        {
            ConfigurationService = configurationService;
            JsonConvert.DefaultSettings = () =>
                {
                    var jsonSerializerSettings = new JsonSerializerSettings
                                                     {
                                                         ContractResolver = new SnakeCasePropertyNamesContractResolver()
                                                     };
                    jsonSerializerSettings.Converters.Add(new SnakeCaseStringEnumConverter());
                    return jsonSerializerSettings;
                };
            WebProxyFactory = webProxyFactory;
            ResourceApi = CreateResourceApi(CreateHttpClient());
            ConfigurationService.AddProxyConfigurationObserver(this);
        }

        #endregion

        #region Public Properties

        public T ResourceApi { protected get; set; }

        public IWebProxyFactory WebProxyFactory { get; set; }

        #endregion

        #region Methods

        protected abstract T CreateResourceApi(HttpClient httpClient);

        protected HttpClient CreateHttpClient()
        {
            var baseAddress = new Uri(ConfigurationService[ConfigurationProperties.BaseUrl]);
            var handler = new HttpClientHandler
                              {
                                  Proxy = WebProxyFactory.CreateFromAppConfiguration(),
                                  UseProxy = true,
                                  AllowAutoRedirect = true,
                              };
            return new HttpClient(handler) { BaseAddress = baseAddress };
        }

        #endregion

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            ResourceApi = CreateResourceApi(CreateHttpClient());
        }
    }
}