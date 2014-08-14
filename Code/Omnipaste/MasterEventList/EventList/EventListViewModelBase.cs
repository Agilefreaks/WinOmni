namespace Omnipaste.MasterEventList.EventList
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.ClippingList;
    using Omnipaste.Event;

    public abstract class EventListViewModelBase : Screen, IEventListViewModel
    {
        #region Constants

        public const int ListSize = 10;

        #endregion

        #region Fields

        private ListViewModelStatusEnum _status;

        #endregion

        #region Constructors and Destructors

        protected EventListViewModelBase(IEventsHandler eventsHandler)
        {
            IncomingEvents = new LimitableBindableCollection<Event>(ListSize);
            IncomingEvents.CollectionChanged += IncomingEventsCollectionChanged;

            Events = new LimitableBindableCollection<IEventViewModel>(ListSize);

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

        public LimitableBindableCollection<Event> IncomingEvents { get; set; }

        public LimitableBindableCollection<IEventViewModel> Events { get; set; }

        public ListViewModelStatusEnum Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        #endregion

        #region Methods

        private void IncomingEventsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status = IncomingEvents.Any() ? ListViewModelStatusEnum.NotEmpty : ListViewModelStatusEnum.Empty;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Events.Add(new EventViewModel());
            }
        }

        #endregion
    }
}