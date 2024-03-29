﻿namespace Omnipaste.Framework.Helpers
{
    public class PhoneNumberMatcher
    {
        public static bool IsMatch(string phoneNumber1, string phoneNumber2)
        {
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
