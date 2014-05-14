using System.Reactive.Subjects;
using OmniCommon.Models;

namespace OmniSync
{
    public interface IWebsocketConnection
    {
        ISubject<OmniMessage>  Connect();

        void Disconnect();

        string RegistrationId { get; set; }
    }
}