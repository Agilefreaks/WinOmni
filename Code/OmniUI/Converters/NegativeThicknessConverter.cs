namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class NegativeThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Thickness(0, 0, 0, 0);
            if (values != null && values.Length == 2)
            {
                var height = (double)values[0];
                var direction = (VerticalAlignment)values[1];
                result = direction == VerticalAlignment.Top
                             ? new Thickness(0, -height, 0, 0)
                             : new Thickness(0, 0, 0, -height);
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
