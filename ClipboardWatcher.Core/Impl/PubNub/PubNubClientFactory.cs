namespace ClipboardWatcher.Core.Impl.PubNub
{
    public class PubNubClientFactory : IPubNubClientFactory
    {
        public Pubnub Create(CommunicationSettings communicationSettings)
        {
            return new Pubnub(communicationSettings.PublishKey,
                     communicationSettings.SubscribeKey,
                     communicationSettings.SecretKey,
                     string.Empty,
                     true);
        }
    }
}
