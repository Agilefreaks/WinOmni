namespace Omnipaste.MasterEventList.AllEventList
{
    using Events.Handlers;
    using Omnipaste.MasterEventList.EventList;

    public class AllEventListViewModel : EventListViewModelBase, IAllEventListViewModel
    {

        public AllEventListViewModel(IEventsHandler eventsHandler) : base(eventsHandler)
        {
        }
    }
}