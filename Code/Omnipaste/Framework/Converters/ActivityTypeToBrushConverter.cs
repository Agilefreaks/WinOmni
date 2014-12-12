namespace Omnipaste.Framework.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Omnipaste.Models;

    public class ActivityTypeToBrushConverter : IValueConverter
    {
        #region Constants

        private const string CallBrush = "CallBrush";

        private const string ClippingBrush = "ClippingBrush";

        private const string MessageBrush = "MessageBrush";

        #endregion

        #region Public Methods and Operators

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string brushName = null;
            if (value is ActivityTypeEnum)
            {
                switch ((ActivityTypeEnum)value)
                {
                    case ActivityTypeEnum.Clipping:
                        brushName = ClippingBrush;
                        break;
                    case ActivityTypeEnum.Message:
                        brushName = MessageBrush;
                        break;
                    case ActivityTypeEnum.Call:
                        brushName = CallBrush;
                        break;
                }
            }

            Brush result = brushName != null
                               ? (SolidColorBrush)Application.Current.FindResource(brushName)
                               : new SolidColorBrush();

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}