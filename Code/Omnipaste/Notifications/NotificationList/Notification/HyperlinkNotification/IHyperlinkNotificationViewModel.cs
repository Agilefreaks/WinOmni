namespace Omnipaste.Notifications.NotificationList.Notification.HyperlinkNotification
{
    using Omnipaste.Notifications.NotificationList.Notification.ClippingNotification;

    public interface IHyperlinkNotificationViewModel : IClippingNotificationViewModel
    {
        void OpenLink();
    }
}