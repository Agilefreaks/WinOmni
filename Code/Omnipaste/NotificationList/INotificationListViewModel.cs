namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Events.Models;
    using Omnipaste.Notification;

    public interface INotificationListViewModel : IScreen, IObserver<Event>
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }
    }
}