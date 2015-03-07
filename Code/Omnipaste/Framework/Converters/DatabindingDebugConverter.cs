namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;

    public class DatabindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}