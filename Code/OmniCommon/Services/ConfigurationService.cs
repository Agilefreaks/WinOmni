using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
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

        public string GetRefreshToken()
        {
            return _configurationProvider.GetValue("refreshToken");
        }

        public string GetTokenType()
        {
            return _configurationProvider.GetValue("tokenType");
        }

        public string GetClientId()
        {
            return ConfigurationManager.AppSettings["client_id"];
        }

        public string GetDeviceIdentifier()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .First();
        }

        public string GetMachineName()
        {
            return System.Environment.MachineName;
        }
    }
}