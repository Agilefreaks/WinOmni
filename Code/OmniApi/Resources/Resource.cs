namespace OmniApi.Resources
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;
    using OmniApi.Support.Converters;
    using OmniApi.Support.Serialization;
    using OmniCommon;
    using OmniCommon.ExtensionMethods;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public abstract class Resource<T> : IDisposable
        where T : class
    {
        #region Fields

        protected readonly IConfigurationService ConfigurationService;

        private readonly IDisposable _proxyChangeSubscription;

        private T _resourceApi;

        #endregion

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
            _proxyChangeSubscription =
                ConfigurationService.SettingsChangedObservable.SubscribeToSettingChange<ProxyConfiguration>(
                    ConfigurationProperties.ProxyConfiguration,
                    OnConfigurationChanged);
        }

        #endregion

        #region Public Properties

        public T ResourceApi
        {
            get
            {
                return _resourceApi ?? (_resourceApi = CreateResourceApi(CreateHttpClient()));
            }
            set
            {
                _resourceApi = value;
            }
        }

        public IWebProxyFactory WebProxyFactory { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            _proxyChangeSubscription.Dispose();
        }

        public void OnConfigurationChanged(ProxyConfiguration proxyConfiguration)
        {
            ResourceApi = CreateResourceApi(CreateHttpClient());
        }

        #endregion

        #region Methods

        protected virtual HttpClient CreateHttpClient()
        {
            var baseAddress = new Uri(ConfigurationService[ConfigurationProperties.BaseUrl]);
            var handler = CreateHttpHandler();
            return new HttpClient(handler) { BaseAddress = baseAddress };
        }

        protected HttpClientHandler CreateHttpHandler()
        {
            var webProxy = WebProxyFactory.CreateFromAppConfiguration();
            return new HttpClientHandler { Proxy = webProxy, UseProxy = webProxy != null, AllowAutoRedirect = true, };
        }

        protected abstract T CreateResourceApi(HttpClient httpClient);

        #endregion
    }
}