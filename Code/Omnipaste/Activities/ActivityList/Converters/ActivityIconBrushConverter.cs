namespace Omnipaste.Activities.ActivityList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Omnipaste.Activities.ActivityList.Activity;
    using Omnipaste.Framework.ExtensionMethods;
    using OmniUI.Framework.Helpers;

    public class ActivityIconBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var contentInfo = (ActivityContentInfo)value;

            var brushName = contentInfo.ContentType.GetBrushName();
            var brush = ResourceHelper.GetByKey<SolidColorBrush>(brushName).Clone();
            if (contentInfo.ContentState == ContentStateEnum.Viewed)
            {
                brush.Opacity = 0.3;
            }
            
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
