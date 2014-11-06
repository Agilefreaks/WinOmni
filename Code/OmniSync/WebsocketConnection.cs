namespace OmniSync
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Newtonsoft.Json.Linq;
    using OmniCommon.Models;
    using WampSharp.Core.Listener;
    using WampSharp.V1;
    using WampSharp.V1.Auxiliary.Client;

    public class WebsocketConnection : IWebsocketConnection
    {
        #region Fields

        protected readonly ReplaySubject<OmniMessage> OmniMessageSubject;

        private readonly IWampChannel<JToken> _channel;

        private IDisposable _channelObserver;

        private IDisposable _connectObserver;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnection(IWampChannel<JToken> channel)
        {
            _channel = channel;
            Monitor = _channel.GetMonitor();
            OmniMessageSubject = new ReplaySubject<OmniMessage>(0);
        }

        #endregion

        #region Public Properties

        public IWampClientConnectionMonitor Monitor { get; private set; }

        public string SessionId
        {
            get
            {
                return Monitor != null ? Monitor.SessionId : null;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<string> Connect()
        {
            DisposeConnectObserver();
            var connectObservable = Observable.Start(_channel.Open).Select(result => Monitor.SessionId);
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

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            return OmniMessageSubject.Subscribe(observer);
        }

        #endregion

        #region Methods

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