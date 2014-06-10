﻿namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        #region Public Properties

        string AccessToken { get; }

        string RefreshToken { get; }

        string TokenType { get; }

        string ClientId { get; }

        string DeviceIdentifier { get; }

        string MachineName { get; }

        #endregion

        #region Public Indexers

        string this[string key] { get; }

        #endregion

        #region Public Methods and Operators

        void Save(string accessToken, string tokenType, string refreshToken);

        #endregion
    }
}