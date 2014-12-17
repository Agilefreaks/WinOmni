namespace Omnipaste.MasterEventList.EventList
{
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using Omnipaste.Event;
    using OmniUI.List;

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
            var eventViewModel = Kernel.Get<IEventViewModel>();
            eventViewModel.Model = entity;

            return eventViewModel;
        }

        #endregion
    }
}