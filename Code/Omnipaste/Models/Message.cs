﻿namespace Omnipaste.Models
{
    using System;
    using Events.Models;
    using Omnipaste.Activity.Models;

    public class Message
    {
        #region Constructors and Destructors

        public Message()
        {
            Time = DateTime.UtcNow;
        }

        public Message(Event @event)
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo(@event);
            Content = @event.Content;
            Source = MessageSourceType.Remote;
        }

        public MessageSourceType Source { get; set; }

        #endregion

        #region Public Properties

        public ContactInfo ContactInfo { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        #endregion
    }
}