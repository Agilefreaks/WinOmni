namespace Omnipaste.Event
{
    using Events.Models;

    public class EventViewModel : DetailsViewModelBase<Event>, IEventViewModel
    {
        #region Constructors and Destructors

        public EventViewModel(Event model)
            : base(model)
        {
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

        public string PhoneNumber
        {
            get
            {
                return Model.PhoneNumber;
            }
        }

        #endregion
    }
}