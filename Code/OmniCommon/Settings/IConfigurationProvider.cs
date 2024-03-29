﻿namespace OmniCommon.Settings
{
    public interface IConfigurationProvider
    {
        #region Public Methods and Operators

        string GetValue(string key);

        bool HasValue(string key);

        #endregion
    }
}