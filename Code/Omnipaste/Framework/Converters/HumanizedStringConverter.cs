namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Humanizer;

    public class HumanizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            var valueType = value.GetType();
            string result = null;
            if (valueType == typeof(string))
            {
                result = ((string)value).Humanize();
            }
            else if (valueType == typeof(DateTime))
            {
                result = ((DateTime)value).Humanize(culture: culture);
            }
            else if (valueType == typeof(DateTime?))
            {
                result = ((DateTime?)value).Value.Humanize(culture: culture);
            }
            else if (valueType == typeof(TimeSpan))
            {
                result = ((TimeSpan)value).Humanize(culture: culture);
            }
            else if (valueType.IsEnum)
            {
                result = ((Enum)value).Humanize();
            }

            return result ?? System.Convert.ToString(value).Humanize();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}