namespace OmniCommon.Interfaces
{
    using System;

    public interface IConfigurationService
    {
        #region Public Properties

        string AccessToken { get; }

        string ClientId { get; }

        bool DebugMode { get; }

        string DeviceIdentifier { get; }

        bool IsSMSSuffixEnabled { get; set; }

        string MachineName { get; }

        ProxyConfiguration ProxyConfiguration { get; }

        string RefreshToken { get; }

        Version Version { get; }

        string WebBaseUrl { get; }

        #endregion

        #region Public Indexers

        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        void AddProxyConfigurationObserver(IProxyConfigurationObserver proxyConfigurationObserver);

        void RemoveProxyConfigurationObserver(IProxyConfigurationObserver proxyConfigurationObserver);

        void ResetAuthSettings();

        void SaveAuthSettings(string accessToken, string refreshToken);

        void SaveProxyConfiguration(ProxyConfiguration proxyConfiguration);

        #endregion
    }
}