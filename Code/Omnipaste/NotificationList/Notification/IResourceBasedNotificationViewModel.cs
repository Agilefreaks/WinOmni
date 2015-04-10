namespace Omnipaste.NotificationList.Notification
{
    public interface IResourceBasedNotificationViewModel<T> : INotificationViewModel
    {
        T Resource { get; set; }
    }
}