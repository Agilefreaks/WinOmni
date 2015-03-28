namespace Contacts.Models
{
    public class PhoneNumberDto
    {
        public PhoneNumberDto(string phoneNumber)
        {
            Number = phoneNumber;
        }

        public PhoneNumberDto()
        {
        }

        public string Number { get; set; }

        public string Type { get; set; }
    }
}