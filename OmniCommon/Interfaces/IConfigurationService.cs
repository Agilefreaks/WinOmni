using OmniCommon.Services;

namespace OmniCommon.Interfaces
{
    public interface IConfigurationService : IStartupTask
    {
        CommunicationSettings CommunicationSettings { get; }

        void LoadCommunicationSettings();

        void UpdateCommunicationChannel(string channel);
    }
}