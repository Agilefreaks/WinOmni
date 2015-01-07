namespace Omnipaste.ActivityDetails.Conversation
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ConversationHeaderStateToProgressBarEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ConversationHeaderStateEnum)value) == ConversationHeaderStateEnum.InitiatingCall;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
