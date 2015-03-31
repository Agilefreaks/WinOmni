namespace OmniUI.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class IntegerToNullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return null;

            var intValue = (int)value;
            var result = intValue == 0 ? null : (int?)intValue;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = 0;
            try
            {
                result = System.Convert.ToInt32(value);
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}