namespace Omnipaste.Notification
{
    using Caliburn.Micro;

    public interface INotificationViewModel : IScreen
    {
        Notifications.Models.Notification Model { get; }
    }
}