namespace Omnipaste.MasterEventList.IncomingCallEventList
{
    using System;
    using Events.Handlers;
    using Events.Models;
    using Ninject;
    using Omnipaste.MasterEventList.EventList;
    using OmniUI.Attributes;

    [UseView(typeof(EventListView))]
    public class IncomingCallEventListViewModel : EventListViewModelBase, IIncomingCallEventListViewModel
    {
        #region Constructors and Destructors

        public IncomingCallEventListViewModel(IEventsHandler eventsHandler, IKernel kernel)
            : base(eventsHandler, kernel)
        {
        }

        #endregion

        #region Public Properties

        public override Func<Event, bool> EntityFilter
        {
            get
            {
                return @event => @event.Type == EventTypeEnum.IncomingCallEvent;
            }
        }

        #endregion
    }
}