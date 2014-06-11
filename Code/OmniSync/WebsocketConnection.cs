namespace OmniSync
{
    using System.Reactive.Subjects;
    using Newtonsoft.Json.Linq;
    using OmniCommon.Models;
    using WampSharp;

    public class WebsocketConnection : IWebsocketConnection
    {
        #region Fields

        private readonly IWampChannel<JToken> _channel;

        private ISubject<OmniMessage> _subject;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnection(IWampChannel<JToken> channel)
        {
            _channel = channel;
        }

        #endregion

        #region Public Properties

        public string RegistrationId { get; set; }

        #endregion

        #region Public Methods and Operators

        public ISubject<OmniMessage> Connect()
        {
            _subject = _channel.GetSubject<OmniMessage>(RegistrationId);
            return _subject;
        }

        public void Disconnect()
        {
            _channel.Close();
        }

        #endregion
    }
}