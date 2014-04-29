using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        CommunicationSettings CommunicationSettings { get; }

        void Initialize();

        void UpdateCommunicationChannel(string channel);

        string GetCommunicationChannel();

        void Save(string accessToken, string tokenType, string refreshToken);

        string GetAccessToken();

        string GetClientId();

        string this[string key] { get; }
    }
}