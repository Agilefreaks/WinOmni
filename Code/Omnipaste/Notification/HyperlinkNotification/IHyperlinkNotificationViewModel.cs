namespace Omnipaste.Notification.HyperlinkNotification
{
    using Omnipaste.Notification.Models;

    public interface IHyperlinkNotificationViewModel : INotificationViewModel
    {
        void OpenLink();

        HyperlinkNotification Model { get; set; }
    }
}