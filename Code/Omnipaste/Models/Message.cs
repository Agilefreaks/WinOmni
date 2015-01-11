namespace Omnipaste.Models
{
    using System;
    using Events.Models;
    using OmniCommon.Helpers;
    using Omnipaste.DetailsViewModel;
    using OmniUI.Models;

    public class Message : IConversationItem
    {
        #region Constructors and Destructors

        public Message()
        {
            Time = TimeHelper.UtcNow;
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public Message(Event @event)
        {
            Time = TimeHelper.UtcNow;
            ContactInfo = new EventContactInfo(@event);
            Content = @event.Content;
            Source = SourceType.Remote;
            UniqueId = @event.UniqueId;
        }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public SourceType Source { get; set; }

        public DateTime Time { get; set; }

        public string UniqueId { get; set; }

        #endregion
    }
}