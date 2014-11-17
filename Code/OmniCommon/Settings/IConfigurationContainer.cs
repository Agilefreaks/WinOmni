namespace OmniCommon.Settings
{
    public interface IConfigurationContainer
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