namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.Notification;

    public interface INotificationListViewModel : IScreen, IObserver<Notifications.Models.Notification>
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }
    }
}