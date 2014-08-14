namespace Omnipaste.MasterEventList.EventList
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.ClippingList;

    public abstract class EventListViewModelBase : IEventListViewModel
    {
        #region Constants

        public const int ListSize = 10;

        #endregion

        #region Constructors and Destructors

        protected EventListViewModelBase(IEventsHandler eventsHandler)
        {
            IncomingEvents = new LimitableBindableCollection<Event>(ListSize);

            EventsHandler = eventsHandler;
            EventsHandler.Where(@event => Filter(@event))
                .Subscribe(@event => IncomingEvents.Add(@event), exception => { });
        }

        #endregion

        #region Public Properties

        public IEventsHandler EventsHandler { get; set; }

        public virtual Func<Event, bool> Filter
        {
            get
            {
                return @event => true;
            }
        }

        public IList<Event> IncomingEvents { get; set; }

        #endregion
    }
}