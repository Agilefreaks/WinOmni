namespace OmniSync
{
    using System;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using OmniCommon.Models;

    public interface IWebsocketConnection : IObservable<WebsocketConnectionStatusEnum>, IObservable<OmniMessage>
    {
        #region Public Properties

        IObservable<WebsocketConnectionStatusEnum> ConnectionObservable { get; }

        string RegistrationId { get; }

        #endregion

        #region Public Methods and Operators

        Task<ISubject<OmniMessage>> Connect();

        void Disconnect();

        #endregion
    }
}