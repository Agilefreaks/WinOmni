namespace Omnipaste.Conversations.ContactList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class BoolToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool canSelectMultipleItems = (bool)value;

            return canSelectMultipleItems ? SelectionMode.Multiple : SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SelectionMode)value == SelectionMode.Multiple;
        }
    }
}
