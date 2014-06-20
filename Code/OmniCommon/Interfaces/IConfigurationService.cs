﻿namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        #region Public Properties

        string AccessToken { get; }

        bool AutoStart { get; set; }

        string ClientId { get; }

        string DeviceIdentifier { get; }

        string MachineName { get; }

        string RefreshToken { get; }

        string TokenType { get; }

        #endregion

        #region Public Indexers

        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        void ResetAuthSettings();

        void SaveAuthSettings(string accessToken, string tokenType, string refreshToken);

        #endregion
    }
}