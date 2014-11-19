namespace OmniCommon.Settings
{
    public interface IConfigurationContainer : IConfigurationProvider
    {
        #region Public Methods and Operators

        bool SetValue(string key, string value);

        #endregion
    }
}