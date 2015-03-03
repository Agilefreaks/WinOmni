namespace SMS.Models
{
    public class SmsMessageDto
    {
        public string Id { get; set; }

        public string DeviceId { get; set; }

        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public int ContactId { get; set; }
        
        public string Type { get; set; }

        public string State { get; set; }
    }
}
