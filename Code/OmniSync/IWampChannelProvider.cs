namespace OmniSync
{
    using Newtonsoft.Json.Linq;
    using WampSharp.V1;

    public interface IWampChannelProvider
    {
        IWampChannel<JToken> GetChannel();
    }
}