namespace OmniHolidays.MessagesWorkspace.Converters
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Windows.Data;
    using global::OmniHolidays.Properties;

    public class TemplateInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resourceManager = new ResourceManager(typeof(Resources));
            return resourceManager.GetString(string.Format("{0}TemplateInfo", value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
