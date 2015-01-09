namespace Omnipaste.MasterEventList.IncomingSmsEventList
{
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using Omnipaste.MasterEventList.EventList;
    using OmniUI.Attributes;

    [UseView(typeof(EventListView))]
    public class IncomingSmsEventListViewModel : EventListViewModelBase, IIncomingSmsEventListViewModel
    {
        #region Constructors and Destructors

        public IncomingSmsEventListViewModel(IEventsHandler eventsHandler, IKernel kernel)
            : base(eventsHandler, kernel)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanHandle(Event @event)
        {
            return @event.Type == EventTypeEnum.IncomingSmsEvent;
        }

        #endregion
    }
}