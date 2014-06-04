namespace Omnipaste.Notification
{
    using Caliburn.Micro;
    using Notifications.Models;

    public class NotificationViewModel : Screen, INotificationViewModel
    {
        public Notification Model { get; private set; }

        public NotificationViewModel(Notification model)
        {
            Model = model;
        }
    }
}