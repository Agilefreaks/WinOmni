﻿namespace Omnipaste.Models
{
    using System;
    using Events.Models;
    using Omnipaste.DetailsViewModel;
    using OmniUI.Models;

    public class Message : IConversationItem
    {
        #region Constructors and Destructors

        public Message()
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo();
            Content = string.Empty;
            Source = SourceType.Local;
        }

        public Message(Event @event)
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo(@event);
            Content = @event.Content;
            Source = SourceType.Remote;
        }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public SourceType Source { get; set; }

        public DateTime Time { get; set; }

        #endregion
    }
}