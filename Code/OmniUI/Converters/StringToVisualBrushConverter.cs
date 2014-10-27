namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public class StringToVisualBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            VisualBrush result = null;
            var iconName = value as string;
            if (!string.IsNullOrWhiteSpace(iconName))
            {
                result = Application.Current.FindResource(iconName) as VisualBrush;
            }

            return result ?? new VisualBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}