namespace Events.Models
{
    public class Event
    {
        public string phone_number { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public EventTypeEnum Type { get; set; }
    }
}