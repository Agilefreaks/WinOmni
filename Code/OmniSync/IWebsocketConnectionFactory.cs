using System.Threading.Tasks;

namespace OmniSync
{
    public interface IWebsocketConnectionFactory
    {
        Task<IWebsocketConnection> Create(string websocketServerUri);
    }
}