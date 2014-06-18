namespace OmniCommon.Interfaces
{
    using System;
    using OmniCommon.Models;

    public interface IHandler
    {
        void Start(IObservable<OmniMessage> omniMessageObservable);

        void Stop();
    }
}