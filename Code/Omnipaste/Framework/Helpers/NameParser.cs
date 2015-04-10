namespace Omnipaste.Framework.Helpers
{
    using System;
    using System.Linq;

    public class NameParser
    {
        public const string NameSeparator = " ";

        public static void Parse(string name, out string firstName, out string lastName)
        {
            firstName = string.Empty;
            lastName = string.Empty;

            var nameParts = string.IsNullOrWhiteSpace(name) ? new string[0] : name.Split(new[] { NameSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length == 1)
            {
                firstName = nameParts.First();
            }
            else if (nameParts.Length > 1)
            {
                firstName = string.Join(NameSeparator, nameParts.Take(nameParts.Length - 1));
                lastName = nameParts.Last();
            }
        }
    }
}
