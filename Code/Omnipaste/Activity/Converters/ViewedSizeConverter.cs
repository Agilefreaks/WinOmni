namespace Omnipaste.Activity.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using OmniUI.Helpers;

    public class ViewedSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var contentInfo = (ActivityContentInfo)value;

            return contentInfo.ContentState == ContentStateEnum.NotViewed
                ? ResourceHelper.GetByKey<double>("ExpandedActivityMaxHeight")
                : ResourceHelper.GetByKey<double>("CollapsedActivityMaxHeight");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
