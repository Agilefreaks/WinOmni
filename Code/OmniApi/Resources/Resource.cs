namespace OmniApi.Resources
{
    using System;
    using System.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Support.Converters;
    using OmniApi.Support.Serialization;
    using OmniCommon;
    using OmniCommon.Interfaces;
    using Refit;

    public abstract class Resource<T>
    {
        #region Fields

        protected readonly T ResourceApi;

        #endregion

        #region Constructors and Destructors

        protected Resource()
        {
            JsonConvert.DefaultSettings = () =>
                {
                    var jsonSerializerSettings = new JsonSerializerSettings()
                                                     {
                                                         ContractResolver = new SnakeCasePropertyNamesContractResolver()
                                                     };
                    jsonSerializerSettings.Converters.Add(new SnakeCaseStringEnumConverter());
                    return jsonSerializerSettings;
                };

            ResourceApi = RestService.For<T>(ConfigurationManager.AppSettings[ConfigurationProperties.BaseUrl]);
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        public Token Token
        {
            get
            {
                return new Token(ConfigurationService.AccessToken, ConfigurationService.RefreshToken);
            }
        }

        public string AccessToken
        {
            get
            {
                return string.Concat("bearer ", Token.AccessToken);
            }
        }

        #endregion

        public IObservable<TModel> Authorize<TModel>(IObservable<TModel> observable)
        {
            return AuthorizationObserver.Authorize(observable, Token);
        }
    }
}