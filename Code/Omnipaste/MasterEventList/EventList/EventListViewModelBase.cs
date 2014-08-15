namespace Omnipaste.MasterEventList.EventList
{
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.Event;
    using Omnipaste.Framework;

    public abstract class EventListViewModelBase : ListViewModelBase<Event, IEventViewModel>, IEventListViewModel
    {
        #region Constructors and Destructors

        protected EventListViewModelBase(IEventsHandler eventsHandler)
            : base(eventsHandler)
        {
        }

        #endregion

        #region Methods

        protected override IEventViewModel CreateViewModel(Event entity)
        {
            return new EventViewModel(entity);
        }

        #endregion
    }
}