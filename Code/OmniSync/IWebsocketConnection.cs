namespace OmniSync
{
    using System;
    using OmniCommon.Models;

    public interface IWebsocketConnection : IObservable<WebsocketConnectionStatusEnum>, IObservable<OmniMessage>
    {
        #region Public Properties

        IObservable<WebsocketConnectionStatusEnum> ConnectionObservable { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<string> Connect();

        void Disconnect();

        #endregion
    }
}