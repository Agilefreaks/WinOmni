namespace OmniClipboard.Messaging
{
    public interface IMessageHandler
    {
        void MessageReceived(string message);

        void MessageSent(string message);

        void MessageSendFailed(string message, string reason);
    }
}