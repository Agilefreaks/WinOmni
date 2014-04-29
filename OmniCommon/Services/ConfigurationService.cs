﻿using System.Configuration;
using OmniCommon.DataProviders;
using OmniCommon.Interfaces;

namespace OmniCommon.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationProvider _configurationProvider;
        
        public CommunicationSettings CommunicationSettings { get; private set; }

        public string this[string key]
        {
            get
            {
                return ConfigurationManager.AppSettings[key];
            }
        }

        public ConfigurationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void UpdateCommunicationChannel(string channel)
        {
            _configurationProvider.SetValue("channel", channel);
            Initialize();
        }

        public string GetCommunicationChannel()
        {
            return CommunicationSettings.Channel;
        }

        public void Save(string accessToken, string tokenType, string refreshToken)
        {
            _configurationProvider.SetValue("accessToken", accessToken);
            _configurationProvider.SetValue("tokenType", tokenType);
            _configurationProvider.SetValue("refreshToken", refreshToken);
        }

        public string GetAccessToken()
        {
            return _configurationProvider.GetValue("accessToken");
        }

        public string GetClientId()
        {
            return ConfigurationManager.AppSettings["client_id"];
        }

        public void Initialize()
        {
            CommunicationSettings = new CommunicationSettings
                {
                    Channel = _configurationProvider.GetValue("channel")
                };
        }
    }
}