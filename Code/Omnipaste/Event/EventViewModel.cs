namespace Omnipaste.Event
{
    using System;
    using Caliburn.Micro;
    using Events.Models;
    using Omnipaste.EventAggregatorMessages;

    public class EventViewModel : DetailsViewModelBase<Event>, IEventViewModel
    {
        public IEventAggregator EventAggregator { get; set; }

        #region Constructors and Destructors

        public EventViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        #endregion

        #region Public Properties

        public string Content
        {
            get
            {
                return Model.Content;
            }
        }

        public EventTypeEnum Type
        {
            get
            {
                return Model.Type;
            }
        }

        public string Title
        {
            get
            {
                return Model.PhoneNumber;
            }
        }

        public DateTime Time
        {
            get
            {
                return Model.Time;
            }
        }

        #endregion

        public void CallBack()
        {
        }

        public void SendSms()
        {
            EventAggregator.PublishOnCurrentThread(new SendSmsMessage{ Recipient = Model.PhoneNumber });
        }
    }
}