namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class WhiteSpaceToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = System.Convert.ToString(value);
            return string.IsNullOrWhiteSpace(s);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}