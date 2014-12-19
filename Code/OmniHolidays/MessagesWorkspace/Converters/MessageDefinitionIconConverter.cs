namespace OmniHolidays.MessagesWorkspace.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;
    using OmniUI.Helpers;

    public class MessageDefinitionIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconNames = new Dictionary<string, string>
                                {
                                    {"Family", "holiday_family_icon"},
                                    {"Friends", "holiday_friends_icon"},
                                    {"Colleagues", "holiday_work_icon"},
                                    {"School", "holiday_school_icon"}
                                };
            var iconKey = iconNames[(string)value];
            
            return ResourceHelper.GetByKey(iconKey);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
