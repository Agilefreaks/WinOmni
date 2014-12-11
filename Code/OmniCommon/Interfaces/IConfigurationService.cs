namespace OmniCommon.Interfaces
{
    using System;
    using OmniCommon.Settings;

    public interface IConfigurationService
    {
        #region Public Properties

        string AccessToken { get; }

        string ClientId { get; }

        bool DebugMode { get; }

        string DeviceIdentifier { get; set; }

        bool IsSMSSuffixEnabled { get; set; }

        string MachineName { get; }

        ProxyConfiguration ProxyConfiguration { get; set; }

        string RefreshToken { get; }

        string AppNameAndVersion { get; }

        Version Version { get; }

        string WebBaseUrl { get; }

        IObservable<SettingsChangedData> SettingsChangedObservable { get; }

        bool IsNewDevice { get; }

        #endregion

        #region Public Indexers

        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        void ResetAuthSettings();

        void SaveAuthSettings(OmnipasteCredentials omnipasteCredentials);

        #endregion
    }
}