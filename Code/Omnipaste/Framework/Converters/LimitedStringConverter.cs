namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class LimitedStringConverter : IValueConverter
    {
        private const int NumberOfCharactersOnLine = 300;

        private const int NumberOfLines = 3;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inputString = string.IsNullOrEmpty(value as string) ? string.Empty : value as string;

            return inputString.Length > (NumberOfLines * NumberOfCharactersOnLine) 
                ? inputString.Substring(0, 3 * 300) 
                : inputString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}