namespace Omnipaste.MasterEventList.IncomingSmsEventList
{
    using System;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.MasterEventList.EventList;
    using OmniUI.Attributes;

    [UseView("Omnipaste.MasterEventList.EventList.EventListView", IsFullyQualifiedName = true)]
    public class IncomingSmsEventListViewModel : EventListViewModelBase, IIncomingSmsEventListViewModel
    {
        #region Constructors and Destructors

        public IncomingSmsEventListViewModel(IEventsHandler eventsHandler)
            : base(eventsHandler)
        {
        }

        #endregion

        #region Public Properties

        public override Func<Event, bool> Filter
        {
            get
            {
                return @event => @event.Type == EventTypeEnum.IncomingSmsEvent;
            }
        }

        #endregion
    }
}