using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService
    {
        CommunicationSettings CommunicationSettings { get; }

        void LoadCommunicationSettings();

        void UpdateCommunicationChannel(string channel);
    }
}