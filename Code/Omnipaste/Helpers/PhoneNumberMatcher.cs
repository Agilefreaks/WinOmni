namespace Omnipaste.Helpers
{
    public class PhoneNumberMatcher
    {
        public static bool IsMatch(string phoneNumber1, string phoneNumber2)
        {
            phoneNumber1 = phoneNumber1 ?? string.Empty;
            phoneNumber2 = phoneNumber2 ?? string.Empty;

            string longest;
            string secondLongest;
            if (phoneNumber1.Length > phoneNumber2.Length)
            {
                longest = phoneNumber1;
                secondLongest = phoneNumber2;
            }
            else
            {
                longest = phoneNumber2;
                secondLongest = phoneNumber1;
            }

            return longest.Contains(secondLongest) && ((double)secondLongest.Length / longest.Length) * 100 > 75d;
        }
    }
}
