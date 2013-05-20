namespace PubNubClipboard
{
    using PubNubWrapper;

    public class PubNubClientFactory : IPubNubClientFactory
    {        
        private const string PublishKey = "pub-c-f6c56076-b928-407d-8e27-462dbf25e722";
        private const string SecretKey = "sec-c-Y2FiOTQzYjEtOTE5NC00YTQ0LWI4YzQtYjYzNjhhNTE1ZTYw";
        private const string SubscribeKey = "sub-c-9f339926-9855-11e2-ac20-12313f022c90";

        public IPubNubClient Create()
        {
            return new Pubnub(PublishKey, SubscribeKey, SecretKey, string.Empty, true);
        }
    }
}
