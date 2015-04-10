namespace Omnipaste.ActivityList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Omnipaste.ActivityList.Activity;
    using Omnipaste.ExtensionMethods;
    using OmniUI.Helpers;

    public class ActivityBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string DefaultActivityBackgroundBrush = "NotificationBackgroundBrush";

            var contentInfo = (ActivityContentInfo)value;
            var brushName = contentInfo.ContentState == ContentStateEnum.Viewing 
                ? contentInfo.ContentType.GetBrushName() 
                : DefaultActivityBackgroundBrush;

            return ResourceHelper.GetByKey<SolidColorBrush>(brushName) ?? new SolidColorBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
