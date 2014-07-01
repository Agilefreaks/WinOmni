namespace Events.Handlers
{
    using System;
    using Events.Models;
    using OmniCommon.Interfaces;
    using OmniCommon.Models;

    public interface IEventsHandler : IObservable<Event>, IHandler, IObserver<OmniMessage>
    {
    }
}