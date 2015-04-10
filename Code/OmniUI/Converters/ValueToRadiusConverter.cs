namespace OmniUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ValueToRadiusConverter : IValueConverter
    {
        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as double?) / 2 ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}