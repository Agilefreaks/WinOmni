namespace Omnipaste.Activity.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Omnipaste.ExtensionMethods;
    using OmniUI.Helpers;

    public class ActivityBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string ViewedActivityBorderBrush = "ScreenBorderBrush";
            var contentInfo = (ActivityContentInfo)value;
            var brushName = contentInfo.ContentState == ContentStateEnum.Viewed
                ? ViewedActivityBorderBrush
                : contentInfo.ContentType.GetBrushName();

            return ResourceHelper.GetByKey<SolidColorBrush>(brushName) ?? new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
