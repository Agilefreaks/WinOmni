namespace OmniCommon.Interfaces
{
    using System;
    using OmniCommon.Models;
    using OmniCommon.Settings;

    public interface IConfigurationService
    {
        #region Public Properties

        string AccessToken { get; }

        string AppNameAndVersion { get; }

        string ClientId { get; }

        bool DebugMode { get; }

        string DeviceId { get; set; }

        //ToDo: This should be removed once the migration to using the  DeviceId id completed - 15.01.2014
        string DeviceIdentifier { get; set; }

        KeyPair DeviceKeyPair { get; set; }

        bool IsNewDevice { get; }

        bool IsSMSSuffixEnabled { get; set; }

        string MachineName { get; }

        ProxyConfiguration ProxyConfiguration { get; set; }

        string RefreshToken { get; }

        IObservable<SettingsChangedData> SettingsChangedObservable { get; }

        UserInfo UserInfo { get; set; }

        Version Version { get; }

        string WebBaseUrl { get; }

        #endregion

        #region Public Indexers

        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        void ClearSettings();

        bool HasSavedValueFor(string propertyName);

        void SaveAuthSettings(OmnipasteCredentials omnipasteCredentials);

        #endregion
    }
}