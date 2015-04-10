namespace Omnipaste.ClippingList.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using OmniUI.Framework.Helpers;

    public class ClippingBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = value as bool?;
            return isSelected.HasValue && isSelected.Value
                       ? ResourceHelper.GetByKey<SolidColorBrush>("ClippingBrush")
                       : ResourceHelper.GetByKey<SolidColorBrush>("PassiveUiElementsBrush");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}