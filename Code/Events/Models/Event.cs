namespace Events.Models
{
    public class Event
    {
        public string PhoneNumber { get; set; }

        public string Content { get; set; }
        
        public EventTypeEnum Type { get; set; }
    }
}