using System.Reactive.Subjects;
using Newtonsoft.Json.Linq;
using OmniCommon.Models;
using WampSharp;

namespace OmniSync
{
    public class WebsocketConnection : IWebsocketConnection
    {
        private readonly IWampChannel<JToken> _channel;

        private ISubject<OmniMessage> _subject;

        public string RegistrationId { get; set; }

        public WebsocketConnection(IWampChannel<JToken> channel)
        {
            _channel = channel;
        }

        public ISubject<OmniMessage> Connect()
        {
            _subject = _channel.GetSubject<OmniMessage>(RegistrationId);
            return _subject;
        }

        public void Disconnect()
        {
            _channel.Close();
        }
    }
}