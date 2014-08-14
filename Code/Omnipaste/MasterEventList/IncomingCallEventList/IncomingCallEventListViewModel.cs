namespace Omnipaste.MasterEventList.IncomingCallEventList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.ClippingList;

    public class IncomingCallEventListViewModel : IIncomingCallEventListViewModel
    {
        public IEventsHandler EventsHandler { get; set; }

        public IList<Event> IncomingEvents { get; set; }

        public const int ListSize = 10;

        public IncomingCallEventListViewModel(IEventsHandler eventsHandler)
        {
            IncomingEvents = new LimitableBindableCollection<Event>(ListSize);
            
            EventsHandler = eventsHandler;
            EventsHandler.Where(@event => @event.Type == EventTypeEnum.IncomingCallEvent)
                .Subscribe(@event => IncomingEvents.Add(@event), exception => { });
        }

    }
}