namespace OmniSync
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Reactive.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using OmniCommon.Models;
    using WampSharp;
    using WampSharp.Auxiliary.Client;

    public class WebsocketConnection : IWebsocketConnection
    {
        #region Fields

        private readonly IWampChannel<JToken> _channel;

        private readonly IWampClientConnectionMonitor _monitor;

        private ISubject<OmniMessage> _subject;

        private IObservable<WebsocketConnectionStatusEnum> _connectionObservable;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnection(IWampChannel<JToken> channel)
        {
            _channel = channel;
            _monitor = Channel.GetMonitor();
        }

        #endregion

        #region Public Properties

        public IObservable<WebsocketConnectionStatusEnum> ConnectionObservable
        {
            get
            {
                return
                    _connectionObservable = _connectionObservable ?? Observable.FromEventPattern(
                            x => _monitor.ConnectionLost += x,
                            x => _monitor.ConnectionLost -= x)
                            .Select(x => WebsocketConnectionStatusEnum.Disconnected)
                            .Concat(
                                Observable.FromEventPattern<WampConnectionEstablishedEventArgs>(
                                    x => _monitor.ConnectionEstablished += x,
                                    x => _monitor.ConnectionEstablished -= x)
                                    .Select(x => WebsocketConnectionStatusEnum.Connected));
            }
        }

        private IWampChannel<JToken> Channel
        {
            get
            {
                return _channel;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<string> Connect()
        {
            return Channel.OpenAsync().ToObservable()
                .Select(
                    result =>
                        {
                            var registrationId = _monitor.SessionId;

                            if (registrationId == null)
                            {
                                throw new Exception("Could not connect to sync server.");
                            }

                            _subject = Channel.GetSubject<OmniMessage>(registrationId);
                            return registrationId;
                        });
        }

        public void Disconnect()
        {
            Channel.Close();
        }

        #endregion

        public IDisposable Subscribe(IObserver<WebsocketConnectionStatusEnum> observer)
        {
            return ConnectionObservable.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}