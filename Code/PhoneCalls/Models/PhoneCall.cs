namespace PhoneCalls.Models
{
    using System;

    public class PhoneCall
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public string ContactName { get; set; }

        public string Type { get; set; }

        public string State { get; set; }
    }
}