﻿namespace Omnipaste.Activities.ActivityList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Omnipaste.Activities.ActivityList.Activity;
    using OmniUI.Framework.Helpers;

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
