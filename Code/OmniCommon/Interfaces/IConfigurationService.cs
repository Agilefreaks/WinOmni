namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        void Save(string accessToken, string tokenType, string refreshToken);

        string GetAccessToken();

        string GetClientId();

        string this[string key] { get; }

        string GetRefreshToken();

        string GetTokenType();

        string GetDeviceIdentifier();

        string GetMachineName();
    }
}