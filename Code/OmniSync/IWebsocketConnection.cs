namespace OmniSync
{
    using System;
    using OmniCommon.Models;
    using WampSharp.Auxiliary.Client;

    public interface IWebsocketConnection : IObservable<OmniMessage>
    {
        #region Public Properties

        IWampClientConnectionMonitor Monitor { get; }

        string SessionId { get; }

        #endregion

        #region Public Methods and Operators

        IObservable<string> Connect();

        void Disconnect();

        #endregion
    }
}