namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class CropStringConverter : IValueConverter
    {
        public const string Ending = "...";
        public const int DefaultMaximumSize = 30;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value != null ? value.ToString() : string.Empty;

            var maxSize = parameter as int? ?? DefaultMaximumSize;
            var indexOfNewLine = str.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            var charCount = indexOfNewLine > -1
                            && indexOfNewLine < maxSize
                                ? indexOfNewLine
                                : maxSize;

            var charArray = str.ToCharArray()
                .Take(charCount)
                .Concat(CreateEnding(charCount, str))
                .ToArray();

            return new String(charArray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string CreateEnding(int charCount, string str)
        {
            return charCount < str.Length ? Ending : string.Empty;
        }
    }
}
