namespace OmniSync
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Newtonsoft.Json.Linq;
    using OmniCommon.Models;
    using WampSharp;
    using WampSharp.Auxiliary.Client;

    public class WebsocketConnection : IWebsocketConnection
    {
        #region Fields

        protected readonly ReplaySubject<OmniMessage> OmniMessageSubject;

        private readonly IWampChannel<JToken> _channel;

        private readonly IWampClientConnectionMonitor _monitor;

        private IDisposable _channelObserver;

        private IDisposable _connectObserver;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnection(IWampChannel<JToken> channel)
        {
            _channel = channel;
            _monitor = _channel.GetMonitor();
            ConnectionObservable = CreateConnectionObservable();
            OmniMessageSubject = new ReplaySubject<OmniMessage>(0);
        }

        #endregion

        #region Public Properties

        public IObservable<WebsocketConnectionStatusEnum> ConnectionObservable { get; private set; }

        #endregion

        #region Public Methods and Operators

        public IObservable<string> Connect()
        {
            DisposeConnectObserver();
            var connectObservable = Observable.Start(_channel.Open).Select(result => _monitor.SessionId);
            _connectObserver = connectObservable.SubscribeOn(Scheduler.Default)
                .Subscribe(OnChannelOpened, _ => DisposeConnectObserver());

            return connectObservable;
        }

        public void Disconnect()
        {
            try
            {
                DisposeChannelObserver();
                DisposeConnectObserver();
                _channel.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public IDisposable Subscribe(IObserver<WebsocketConnectionStatusEnum> observer)
        {
            return ConnectionObservable.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            return OmniMessageSubject.Subscribe(observer);
        }

        #endregion

        #region Methods

        private IObservable<WebsocketConnectionStatusEnum> CreateConnectionObservable()
        {
            var connectionLostObservable =
                Observable.FromEventPattern(x => _monitor.ConnectionLost += x, x => _monitor.ConnectionLost -= x)
                    .Select(x => WebsocketConnectionStatusEnum.Disconnected);

            var connectionEstablishedObservable =
                Observable.FromEventPattern<WampConnectionEstablishedEventArgs>(
                    x => _monitor.ConnectionEstablished += x,
                    x => _monitor.ConnectionEstablished -= x).Select(x => WebsocketConnectionStatusEnum.Connected);

            var connectionErrorObservable =
                Observable.FromEventPattern<WampConnectionErrorEventArgs>(
                    x => _monitor.ConnectionError += x,
                    x => _monitor.ConnectionError -= x).Select(x => WebsocketConnectionStatusEnum.Disconnected);

            return
                ConnectionObservable =
                ConnectionObservable
                ?? connectionLostObservable.Merge(connectionEstablishedObservable).Merge(connectionErrorObservable);
        }

        private void DisposeChannelObserver()
        {
            if (_channelObserver == null)
            {
                return;
            }

            _channelObserver.Dispose();
            _channelObserver = null;
        }

        private void DisposeConnectObserver()
        {
            if (_connectObserver == null)
            {
                return;
            }

            _connectObserver.Dispose();
            _connectObserver = null;
        }

        private void OnChannelOpened(string sessionId)
        {
            DisposeConnectObserver();
            DisposeChannelObserver();
            _channelObserver = _channel.GetSubject<OmniMessage>(sessionId).Subscribe(OmniMessageSubject);
        }

        #endregion
    }
}