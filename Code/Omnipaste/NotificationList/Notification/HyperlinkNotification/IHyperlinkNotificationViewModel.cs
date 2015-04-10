namespace Omnipaste.NotificationList.Notification.HyperlinkNotification
{
    using Omnipaste.NotificationList.Notification.ClippingNotification;

    public interface IHyperlinkNotificationViewModel : IClippingNotificationViewModel
    {
        void OpenLink();
    }
}