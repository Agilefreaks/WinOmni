namespace OmniSync
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using WampSharp.Core.Listener;
    using WampSharp.V1.Auxiliary.Client;

    public class WebSocketMonitor : IWebSocketMonitor
    {
        #region Fields

        private readonly ReplaySubject<WebSocketConnectionStatusEnum> _subject;

        private IDisposable _connectionObserver;

        #endregion

        #region Constructors and Destructors

        public WebSocketMonitor()
        {
            _subject = new ReplaySubject<WebSocketConnectionStatusEnum>(0);
        }

        #endregion

        #region Public Properties

        public IObservable<WebSocketConnectionStatusEnum> ConnectionObservable
        {
            get
            {
                return _subject;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Start(IWebsocketConnection websocketConnection)
        {
            var wampMonitor = websocketConnection.Monitor;
            var connectionLostObservable =
                Observable.FromEventPattern(
                    x => wampMonitor.ConnectionLost += x,
                    x => wampMonitor.ConnectionLost -= x).Select(x => WebSocketConnectionStatusEnum.Disconnected);

            var connectionEstablishedObservable =
                Observable.FromEventPattern<WampConnectionEstablishedEventArgs>(
                    x => wampMonitor.ConnectionEstablished += x,
                    x => wampMonitor.ConnectionEstablished -= x).Select(x => WebSocketConnectionStatusEnum.Connected);

            var connectionErrorObservable =
                Observable.FromEventPattern<WampConnectionErrorEventArgs>(
                    x => wampMonitor.ConnectionError += x,
                    x => wampMonitor.ConnectionError -= x).Select(x => WebSocketConnectionStatusEnum.Disconnected);

            _connectionObserver =
                connectionLostObservable.Merge(connectionEstablishedObservable)
                    .Merge(connectionErrorObservable)
                    .Subscribe(_subject);
        }

        public void Stop()
        {
            if (_connectionObserver != null)
            {
                _connectionObserver.Dispose();
            }
        }

        #endregion
    }
}