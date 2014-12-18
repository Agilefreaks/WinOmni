namespace Omnipaste.MasterEventList.AllEventList
{
    using Events.Handlers;
    using Ninject;
    using Omnipaste.MasterEventList.EventList;
    using OmniUI.Attributes;

    [UseView(typeof(EventListView))]
    public class AllEventListViewModel : EventListViewModelBase, IAllEventListViewModel
    { 
        public AllEventListViewModel(IEventsHandler eventsHandler, IKernel kernel) : base(eventsHandler, kernel)
        {
        }
    }
}