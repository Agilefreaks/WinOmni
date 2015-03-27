namespace Omnipaste.Models
{
    public class PhoneNumber
    {
        public PhoneNumber()
        {
        }

        public PhoneNumber(string number)
        {
            Number = number;
        }

        public string Number { get; set; }

        public string Type { get; set; }
    }
}