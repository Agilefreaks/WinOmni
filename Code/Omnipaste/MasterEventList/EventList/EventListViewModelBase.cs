namespace Omnipaste.MasterEventList.EventList
{
    using System;
    using System.Reactive.Linq;
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using OmniCommon.ExtensionMethods;
    using Omnipaste.Event;
    using OmniUI.List;

    public abstract class EventListViewModelBase : ListViewModelBase<Event, IEventViewModel>, IEventListViewModel
    {
        #region Fields

        protected readonly IKernel Kernel;

        private readonly IDisposable _itemAddedSubscription;

        #endregion

        #region Constructors and Destructors

        protected EventListViewModelBase(IEventsHandler eventsHandler, IKernel kernel)
        {
            Kernel = kernel;
            _itemAddedSubscription = eventsHandler.Where(CanHandle).SubscribeAndHandleErrors(AddItem);
        }

        public override void Dispose()
        {
            _itemAddedSubscription.Dispose();
            base.Dispose();
        }

        #endregion

        #region Public Methods and Operators

        public abstract bool CanHandle(Event @event);

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