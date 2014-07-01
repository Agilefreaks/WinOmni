namespace Notifications.Models
{
    public enum NotificationTypeEnum
    {
        IncomingCallNotification
    }

    public class Notification
    {
        public NotificationTypeEnum Type { get; set; }

        public string PhoneNumber { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}