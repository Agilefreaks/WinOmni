namespace Omnipaste.Helpers
{
    using System.Text.RegularExpressions;

    public class PhoneNumberMatcher
    {
        public static bool IsMatch(string phoneNumber1, string phoneNumber2)
        {
            phoneNumber1 = NormalizePhoneNumber(phoneNumber1);
            phoneNumber2 = NormalizePhoneNumber(phoneNumber2);

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

        private static string NormalizePhoneNumber(string phoneNumber)
        {
            phoneNumber = phoneNumber ?? string.Empty;
            return Regex.Replace(phoneNumber, "[^+0-9]", "");
        }
    }
}
