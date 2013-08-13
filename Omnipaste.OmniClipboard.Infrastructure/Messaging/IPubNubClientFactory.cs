using PubNubWrapper;

namespace Omnipaste.OmniClipboard.Infrastructure.Messaging
{
    public interface IPubNubClientFactory
    {
        IPubNubClient Create();
    }
}