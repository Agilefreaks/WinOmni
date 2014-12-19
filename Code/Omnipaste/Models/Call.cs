namespace Omnipaste.Models
{
    using System;
    using Events.Models;
    using Omnipaste.DetailsViewModel;
    using OmniUI.Models;

    public class Call : IConversationItem
    {
        #region Constructors and Destructors

        public Call()
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo();
            Source = SourceType.Local;
        }

        public Call(Event @event)
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo(@event);
            Source = SourceType.Remote;
        }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        public SourceType Source { get; set; }

        public DateTime Time { get; set; }

        #endregion
    }
}