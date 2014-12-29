namespace OmniCommon.ExtensionMethods
{
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.Trim() == string.Empty;
        }

        public static string RemoveDiacritics(this string stringToStrip)
        {
            var normalizedFormDString = stringToStrip.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var t in from character in normalizedFormDString
                              let unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character)
                              where unicodeCategory != UnicodeCategory.NonSpacingMark
                              select character)
            {
                stringBuilder.Append(t);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}