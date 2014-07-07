namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using Hardcodet.Wpf.TaskbarNotification;

    public class ShowBubbleNotificationBehavior
    {
        public static readonly DependencyProperty BaloonInfoProperty = DependencyProperty.RegisterAttached(
            "BaloonInfo",
            typeof(BaloonNotificationInfo),
            typeof(ShowBubbleNotificationBehavior),
            new FrameworkPropertyMetadata(new BaloonNotificationInfo(), OnBaloonInfoPropertyChanged));

        public static BaloonNotificationInfo GetBaloonInfo(TaskbarIcon control)
        {
            return (BaloonNotificationInfo)control.GetValue(BaloonInfoProperty);
        }

        public static void SetBaloonInfo(TaskbarIcon control, BaloonNotificationInfo value)
        {
            control.SetValue(BaloonInfoProperty, value);
        }

        private static void OnBaloonInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var baloonNotificationInfo = e.NewValue as BaloonNotificationInfo;
            var taskbarIcon = d as TaskbarIcon;
            
            if (baloonNotificationInfo != null && taskbarIcon != null)
            {
                taskbarIcon.ShowBalloonTip(baloonNotificationInfo.Title, baloonNotificationInfo.Message, BalloonIcon.Info);
            }
        }
    }
}