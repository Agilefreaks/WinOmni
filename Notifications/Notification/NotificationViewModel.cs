using Caliburn.Micro;

namespace Notifications.Notification
{
    public class NotificationViewModel : Screen, INotificationViewModel
    {
        public Models.Notification Model { get; set; }
    }
}