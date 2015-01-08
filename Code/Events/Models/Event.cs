namespace Events.Models
{
    using System;

    public class Event
    {
        #region Constructors and Destructors

        public Event()
        {
            Time = DateTime.Now;
            UniqueId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Public Properties

        public string ContactName { get; set; }

        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime Time { get; set; }

        public EventTypeEnum Type { get; set; }

        public string UniqueId { get; set; }

        #endregion
    }
}