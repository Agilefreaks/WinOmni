namespace OmniDebug
{
    using Newtonsoft.Json.Linq;
    using OmniCommon.Models;
    using OmniSync;
    using WampSharp;

    public class WebsocketConnectionWrapper : WebsocketConnection
    {
        #region Constructors and Destructors

        public WebsocketConnectionWrapper(IWampChannel<JToken> channel)
            : base(channel)
        {
        }

        #endregion

        #region Public Methods and Operators

        public void SimulateMessage(OmniMessage omniMessage)
        {
            OmniMessageSubject.OnNext(omniMessage);
        }

        #endregion
    }
}