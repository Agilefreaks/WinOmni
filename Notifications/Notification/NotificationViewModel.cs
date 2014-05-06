using Caliburn.Micro;

namespace Notifications.Notification
{
    public class NotificationViewModel : Screen, INotificationViewModel
    {
        public Models.Notification Item { get; set; }
    }
}