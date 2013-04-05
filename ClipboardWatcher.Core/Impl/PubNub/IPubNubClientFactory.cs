namespace ClipboardWatcher.Core.Impl.PubNub
{
    public interface IPubNubClientFactory
    {
        Pubnub Create(CommunicationSettings communicationSettings);
    }
}