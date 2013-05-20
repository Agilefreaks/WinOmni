namespace Omnipaste.Services
{
    public interface IConfigurationProvider
    {
        string GetValue(string key);

        bool SetValue(string key, string value);
    }
}