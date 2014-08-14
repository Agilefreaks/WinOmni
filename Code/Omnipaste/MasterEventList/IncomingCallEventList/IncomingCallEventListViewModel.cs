namespace Omnipaste.MasterEventList.IncomingCallEventList
{
    using System;
    using Events.Handlers;
    using Events.Models;
    using Omnipaste.MasterEventList.EventList;

    public class IncomingCallEventListViewModel : EventListViewModelBase, IIncomingCallEventListViewModel
    {
        #region Constructors and Destructors

        public IncomingCallEventListViewModel(IEventsHandler eventsHandler)
            : base(eventsHandler)
        {
        }

        #endregion

        #region Public Properties

        public override Func<Event, bool> Filter
        {
            get
            {
                return @event => @event.Type == EventTypeEnum.IncomingCallEvent;
            }
        }

        #endregion
    }
}