namespace Events.Api.Resources.v1
{
    using System;
    using global::Events.Models;

    public interface IEvents
    {
        IObservable<Event> Last();
    }
}