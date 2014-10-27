namespace OmniDebug
{
    using System;
    using OmniCommon.Models;
    using OmniSync;

    public class WebsocketConnectionWrapper : IWebsocketConnectionWrapper
    {
        #region Fields

        private readonly OmniMessageSubject _omniMessageSubject;

        private readonly IWebsocketConnection _existingConnection;

        private IDisposable _connectionObserver;

        #endregion

        #region Constructors and Destructors

        public WebsocketConnectionWrapper(IWebsocketConnection existingConnection)
        {
            _existingConnection = existingConnection;
            _omniMessageSubject = new OmniMessageSubject();
        }

        #endregion

        #region Public Properties

        public IObservable<WebsocketConnectionStatusEnum> ConnectionObservable
        {
            get
            {
                return _existingConnection.ConnectionObservable;
            }
        }

        #endregion

        #region Public Methods and Operators

        public IObservable<string> Connect()
        {
            return _existingConnection.Connect();
        }

        public void Disconnect()
        {
            _existingConnection.Disconnect();
        }

        public void SimulateMessage(OmniMessage omniMessage)
        {
            _omniMessageSubject.OnNext(omniMessage);
        }

        public IDisposable Subscribe(IObserver<WebsocketConnectionStatusEnum> observer)
        {
            return _existingConnection.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<OmniMessage> observer)
        {
            SubscribeWrapper();
            return _omniMessageSubject.Subscribe(observer);
        }

        private void SubscribeWrapper()
        {
            if (_connectionObserver == null)
            {
                _connectionObserver = _existingConnection.Subscribe(_omniMessageSubject);
            }
        }

        #endregion
    }
}