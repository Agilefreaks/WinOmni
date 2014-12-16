namespace Omnipaste.MasterEventList.EventList
{
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using Omnipaste.Event;
    using OmniUI.List;

    public abstract class EventListViewModelBase : ListViewModelBase<Event, IEventViewModel>, IEventListViewModel
    {
        #region Fields

        protected readonly IKernel Kernel;

        #endregion

        #region Constructors and Destructors

        protected EventListViewModelBase(IEventsHandler eventsHandler, IKernel kernel)
            : base(eventsHandler)
        {
            Kernel = kernel;
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