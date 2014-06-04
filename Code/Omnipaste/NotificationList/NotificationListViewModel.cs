namespace Omnipaste.NotificationList
{
    using System;
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Notifications.Handlers;
    using Notifications.Models;
    using Omnipaste.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel
    {
        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        public NotificationListViewModel(INotificationsHandler notificationsHandler)
        {
            Notifications = new ObservableCollection<INotificationViewModel>();
            notificationsHandler.Subscribe(this);
        }

        public void OnNext(Notification notification)
        {
            notification.Title = string.Concat("Incoming call from ", notification.phone_number);

            Execute.OnUIThread(() => Notifications.Add(new NotificationViewModel { Model = notification }));
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}