namespace Omnipaste.NotificationList
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Notification;

    public interface INotificationListViewModel : IScreen
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }
    }
}