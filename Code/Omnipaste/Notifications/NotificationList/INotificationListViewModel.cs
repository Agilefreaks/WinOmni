namespace Omnipaste.Notifications.NotificationList
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Framework.EventAggregatorMessages;
    using Omnipaste.Notifications.NotificationList.Notification;

    public interface INotificationListViewModel : IScreen, IHandle<DismissNotification>
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }

        double Height { get; set; }

        void Show();

        void UpdateNotificationSubscriptions(bool pauseNotifications);
    }
}