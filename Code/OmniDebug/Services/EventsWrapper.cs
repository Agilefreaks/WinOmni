namespace OmniDebug.Services
{
    using System;
    using System.Reactive.Linq;
    using Events.Api.Resources.v1;
    using Events.Models;

    public class EventsWrapper : IEventsWrapper
    {
        private readonly IEvents _events;

        private Event _eventToPush;

        public EventsWrapper(IEvents events)
        {
            _events = events;
        }

        public void MockLast(Event @event)
        {
            _eventToPush = @event;
        }

        public IObservable<Event> Last()
        {
            IObservable<Event> observable;
            if (_eventToPush != null)
            {
                observable = new[] { _eventToPush }.ToObservable();
                _eventToPush = null;
            }
            else
            {
                observable = _events.Last();
            }

            return observable;
        }
    }
}