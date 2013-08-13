using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        CommunicationSettings CommunicationSettings { get; }

        ApiConfig ApiConfig { get; set; }

        void Initialize();

        void UpdateCommunicationChannel(string channel);
    }
}