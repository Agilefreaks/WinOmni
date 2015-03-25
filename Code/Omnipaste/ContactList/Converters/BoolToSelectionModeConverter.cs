namespace Omnipaste.ContactList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class BoolToSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool canSelectMultipleItems = (bool)value;

            return canSelectMultipleItems
                ? System.Windows.Controls.SelectionMode.Multiple
                : System.Windows.Controls.SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}