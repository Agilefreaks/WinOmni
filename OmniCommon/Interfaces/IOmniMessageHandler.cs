namespace OmniCommon.Interfaces
{
    using System;
    using OmniCommon.Models;

    public interface IOmniMessageHandler : IObserver<OmniMessage>, IDisposable
    {
        void SubscribeTo(IObservable<OmniMessage> observable);
    }
}