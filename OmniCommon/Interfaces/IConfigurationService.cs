using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        CommunicationSettings CommunicationSettings { get; }

        void Initialize();

        void UpdateCommunicationChannel(string channel);

        string GetCommunicationChannel();

        string GetClientId();

        string this[string key] { get; }
    }
}