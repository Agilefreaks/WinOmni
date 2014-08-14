namespace Omnipaste.MasterEventList.AllEventList
{
    using Events.Handlers;
    using Omnipaste.Framework.Attributes;
    using Omnipaste.MasterEventList.EventList;

    [UseView("Omnipaste.MasterEventList.EventList.EventListView", IsFullyQualifiedName = true)]
    public class AllEventListViewModel : EventListViewModelBase, IAllEventListViewModel
    {
        public AllEventListViewModel(IEventsHandler eventsHandler) : base(eventsHandler)
        {
        }
    }
}