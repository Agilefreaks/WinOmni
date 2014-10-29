namespace Omnipaste.Framework.Behaviours
{
    using System.Windows;
    using Hardcodet.Wpf.TaskbarNotification;

    public class ShowBubbleNotificationBehavior
    {
        public static readonly DependencyProperty BalloonInfoProperty = DependencyProperty.RegisterAttached(
            "BalloonInfo",
            typeof(BalloonNotificationInfo),
            typeof(ShowBubbleNotificationBehavior),
            new FrameworkPropertyMetadata(new BalloonNotificationInfo(), OnBalloonInfoPropertyChanged));

        public static BalloonNotificationInfo GetBalloonInfo(TaskbarIcon control)
        {
            return (BalloonNotificationInfo)control.GetValue(BalloonInfoProperty);
        }

        public static void SetBalloonInfo(TaskbarIcon control, BalloonNotificationInfo value)
        {
            control.SetValue(BalloonInfoProperty, value);
        }

        private static void OnBalloonInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var balloonNotificationInfo = e.NewValue as BalloonNotificationInfo;
            var taskbarIcon = d as TaskbarIcon;
            
            if (balloonNotificationInfo != null && taskbarIcon != null)
            {
                taskbarIcon.ShowBalloonTip(balloonNotificationInfo.Title, balloonNotificationInfo.Message, BalloonIcon.Info);
            }
        }
    }
}