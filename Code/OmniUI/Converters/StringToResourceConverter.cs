namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using OmniUI.Helpers;

    public class StringToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            var iconName = value as string;
            if (!string.IsNullOrWhiteSpace(iconName))
            {
                result = ResourceHelper.GetByKey(iconName);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}