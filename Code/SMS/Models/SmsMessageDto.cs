namespace SMS.Models
{
    using System.Collections.Generic;

    public class SmsMessageDto
    {
        private List<string> _phoneNumberList;

        public string Id { get; set; }

        public string DeviceId { get; set; }

        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public List<string> PhoneNumberList
        {
            get
            {
                if (!string.IsNullOrEmpty(PhoneNumber) && _phoneNumberList == null)
                {
                    _phoneNumberList = new List<string> { PhoneNumber };
                }

                return _phoneNumberList;
            }
            set
            {
                _phoneNumberList = value;
            }
        }

        public int? ContactId { get; set; }
        
        public string Type { get; set; }

        public string State { get; set; }
    }
}
