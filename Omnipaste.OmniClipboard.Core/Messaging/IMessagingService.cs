namespace Omnipaste.OmniClipboard.Core.Messaging
{
    public interface IMessagingService
    {
        bool Connect(string channel, IMessageHandler messageHandler);

        void Disconnect(string channel);

        void SendAsync(string channel, string message, IMessageHandler messageHandler);
    }
}