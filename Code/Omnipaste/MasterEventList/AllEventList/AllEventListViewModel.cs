namespace Omnipaste.MasterEventList.AllEventList
{
    using System;
    using System.Collections.Generic;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.ClippingList;

    public class AllEventListViewModel : IAllEventListViewModel
    {
        public IEventsHandler EventsHandler { get; set; }

        public IList<Event> IncomingEvents { get; set; }

        public const int ListSize = 10;

        public AllEventListViewModel(IEventsHandler eventsHandler)
        {
            IncomingEvents = new LimitableBindableCollection<Event>(ListSize);
            
            EventsHandler = eventsHandler;
            EventsHandler.Subscribe(
                @event => IncomingEvents.Add(@event),
                exception => {});
        }
    }
}