namespace Omnipaste.Models
{
    using Events.Models;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using OmniUI.Models;

    public class Call : BaseModel, IConversationItem
    {
        #region Constructors and Destructors

        public Call()
        {
            Time = TimeHelper.UtcNow;
            ContactInfo = new ContactInfo();
            Source = SourceType.Local;
        }

        public Call(Event @event)
        {
            Time = TimeHelper.UtcNow;
            ContactInfo = new EventContactInfo(@event);
            Source = SourceType.Remote;
            UniqueId = @event.UniqueId;
            Content = @event.Content;
        }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        public SourceType Source { get; set; }

        public string Content { get; set; }

        #endregion
    }
}