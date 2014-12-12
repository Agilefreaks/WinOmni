namespace Omnipaste.Models
{
    using System;
    using Events.Models;

    public class Call : IHaveTimestamp, IHaveContactInfo
    {
        public Call()
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo();
        }

        public Call(Event @event)
        {
            Time = DateTime.UtcNow;
            ContactInfo = new ContactInfo(@event);
        }

        public DateTime Time { get; set; }

        public ContactInfo ContactInfo { get; set; }
    }
}