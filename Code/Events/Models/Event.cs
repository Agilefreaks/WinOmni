namespace Events.Models
{
    using System;

    public class Event
    {
        public string PhoneNumber { get; set; }

        public string Content { get; set; }
        
        public EventTypeEnum Type { get; set; }

        public DateTime Time { get; set; }

        public Event()
        {
            Time = DateTime.Now;
        }
    }
}