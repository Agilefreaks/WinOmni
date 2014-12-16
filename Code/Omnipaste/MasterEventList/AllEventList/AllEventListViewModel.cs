namespace Omnipaste.MasterEventList.AllEventList
{
    using Events.Handlers;
    using Ninject;
    using Omnipaste.MasterEventList.EventList;
    using OmniUI.Attributes;

    [UseView("Omnipaste.MasterEventList.EventList.EventListView", IsFullyQualifiedName = true)]
    public class AllEventListViewModel : EventListViewModelBase, IAllEventListViewModel
    { 
        public AllEventListViewModel(IEventsHandler eventsHandler, IKernel kernel) : base(eventsHandler, kernel)
        {
        }
    }
}