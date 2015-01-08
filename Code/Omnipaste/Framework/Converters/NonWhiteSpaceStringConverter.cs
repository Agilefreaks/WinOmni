namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NonWhiteSpaceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToString(value).Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}