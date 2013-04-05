namespace ClipboardWatcher.Core.Impl.PubNub
{
    public class PubNubClientFactory : IPubNubClientFactory
    {
        private const string PublishKey = "pub-c-4022c9ea-2a2d-4e82-a47f-236087d30af3";
        private const string SecretKey = "sec-c-OWJlYzIwNGMtN2VhZC00YjYwLThmMzAtOTRjZjNjY2YxMTI0";
        private const string SubscribeKey = "sub-c-bbc72840-830c-11e2-9881-12313f022c90";

        public Pubnub Create()
        {
            return new Pubnub(PublishKey, SubscribeKey, SecretKey, string.Empty, true);
        }
    }
}
