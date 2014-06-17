namespace OmniCommon.DataProviders
{
    public interface IConfigurationProvider
    {
        #region Public Indexers

        string this[string key] { get; set; }

        #endregion

        #region Public Methods and Operators

        string GetValue(string key);

        T GetValue<T>(string key, T defaultValue);

        bool SetValue(string key, string value);

        #endregion
    }
}