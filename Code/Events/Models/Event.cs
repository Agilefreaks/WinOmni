namespace Events.Models
{
    using System;

    public class Event
    {
        #region Constructors and Destructors

        public Event()
        {
            Time = DateTime.Now;
        }

        #endregion

        #region Public Properties

        public string ContactName { get; set; }

        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime Time { get; set; }

        public EventTypeEnum Type { get; set; }

        #endregion
    }
}