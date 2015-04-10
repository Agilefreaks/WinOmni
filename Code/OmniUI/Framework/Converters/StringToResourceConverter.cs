namespace OmniUI.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using OmniUI.Framework.Helpers;

    public class StringToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;
            var resourceKey = value as string;
            var defaultResourceKey = parameter as string;

            if (!string.IsNullOrWhiteSpace(resourceKey))
            {
                result = ResourceHelper.GetByKey(resourceKey);
            } 
            else if (!string.IsNullOrEmpty(defaultResourceKey))
            {
                result = ResourceHelper.GetByKey(defaultResourceKey);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}