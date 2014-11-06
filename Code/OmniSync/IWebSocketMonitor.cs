namespace OmniSync
{
    using System;

    public interface IWebSocketMonitor
    {
        IObservable<WebSocketConnectionStatusEnum> ConnectionObservable { get; }

        void Stop();

        void Start(IWebsocketConnection websocketConnection);
    }
}