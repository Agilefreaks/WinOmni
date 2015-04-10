namespace OmniUI.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var url = (string)value;
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) ? new Uri((string)value) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
