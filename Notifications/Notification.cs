namespace Notifications
{
    public enum NotificationTypeEnum
    {
        IncomingCallNotification
    }
    public class Notification
    {
        NotificationTypeEnum Type { get; set; }
    }
}