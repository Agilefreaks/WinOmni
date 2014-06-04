namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public class NotificationViewModel : Screen, INotificationViewModel
    {
        public Notifications.Models.Notification Model { get; set; }
    }
}