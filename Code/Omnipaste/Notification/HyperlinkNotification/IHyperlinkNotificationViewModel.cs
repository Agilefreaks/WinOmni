namespace Omnipaste.Notification.HyperlinkNotification
{
    using Omnipaste.Notification.ClippingNotification;

    public interface IHyperlinkNotificationViewModel : IClippingNotificationViewModel
    {
        void OpenLink();
    }
}