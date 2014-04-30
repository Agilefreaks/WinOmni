using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        CommunicationSettings CommunicationSettings { get; }

        void Initialize();
        
        void Save(string accessToken, string tokenType, string refreshToken);

        string GetAccessToken();

        string GetClientId();

        string this[string key] { get; }

        string GetRefreshToken();

        string GetTokenType();
    }
}