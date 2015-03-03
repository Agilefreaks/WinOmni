namespace PhoneCalls.Models
{
    public class PhoneCallDto
    {
        public string Id { get; set; }

        public string DeviceId { get; set; }

        public string Number { get; set; }

        public string ContactName { get; set; }

        public int? ContactId { get; set; }

        public string Type { get; set; }

        public string State { get; set; }
    }
}