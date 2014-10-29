namespace OmniApi.Resources
{
    using System;
    using System.Diagnostics;
    using BugFreak;
    using Newtonsoft.Json;
    using Ninject;
    using OmniApi.Models;
    using OmniApi.Support.Converters;
    using OmniApi.Support.Serialization;
    using OmniCommon.Interfaces;

    public abstract class Resource<T>
    {
        #region Fields

        public T ResourceApi { protected get; set; }

        #endregion

        #region Constructors and Destructors

        protected Resource(T resourceApi)
        {
            JsonConvert.DefaultSettings = () =>
                {
                    var jsonSerializerSettings = new JsonSerializerSettings
                                                     {
                                                         ContractResolver = new SnakeCasePropertyNamesContractResolver()
                                                     };
                    jsonSerializerSettings.Converters.Add(new SnakeCaseStringEnumConverter());
                    return jsonSerializerSettings;
                };
            ResourceApi = resourceApi;
        }

        #endregion

        #region Public Properties

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public ISessionManager SessionManager { get; set; }

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
                if (string.IsNullOrEmpty(Token.AccessToken))
                {
                    var callingMethodName = new StackFrame(1).GetMethod().Name;
                    ReportingService.Instance.BeginReport(new Exception(string.Format("AccessToken is empty when calling {0}",callingMethodName)));
                }
                return string.Concat("bearer ", Token.AccessToken);
            }
        }

        #endregion

        public IObservable<TModel> Authorize<TModel>(IObservable<TModel> observable)
        {
            return AuthorizationObserver.Authorize(observable, SessionManager, Token);
        }
    }
}