namespace Omnipaste.ContactList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using OmniUI.Helpers;

    public class ContactInfoBackroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = value as bool?;
            return isSelected.HasValue && isSelected.Value
                       ? ResourceHelper.GetByKey<SolidColorBrush>("MessageBrush")
                       : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
