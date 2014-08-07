namespace Omnipaste.Notification.HyperlinkNotification
{
    using Omnipaste.Notification.Models;

    public interface IHyperlinkNotificationViewModel : INotificationViewModel
    {
        HyperlinkNotification Model { get; set; }

        void OpenLink();
    }
}