using System.Collections.ObjectModel;
using Caliburn.Micro;
using Notifications.Notification;

namespace Notifications.NotificationList
{
    public interface INotificationListViewModel : IScreen
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }

        void Handle(Models.Notification message);
    }
}