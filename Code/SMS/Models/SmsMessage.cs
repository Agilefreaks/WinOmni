namespace SMS.Models
{
    using System;

    public class SmsMessage
    {
        public string Id { get; set; }

        public DateTime Time { get; set; }

        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public string ContactName { get; set; }

        public string Type { get; set; }

        public string State { get; set; }
    }
}
