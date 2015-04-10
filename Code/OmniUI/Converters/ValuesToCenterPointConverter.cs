namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    public class ValuesToCenterPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var convertedValues = values.OfType<Double>().ToArray();
            var result = new Point(convertedValues.First() / 2, convertedValues.Last() / 2);
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}