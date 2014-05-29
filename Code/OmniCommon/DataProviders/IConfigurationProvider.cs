namespace OmniCommon.DataProviders
{
    public interface IConfigurationProvider
    {
        string GetValue(string key);

        bool SetValue(string key, string value);

        string this[string key] { get; set; }
    }
}