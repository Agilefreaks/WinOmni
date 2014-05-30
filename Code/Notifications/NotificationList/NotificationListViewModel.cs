namespace Notifications.NotificationList
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Notifications.Notification;

    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel, IHandle<Models.Notification>
    {
        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        public NotificationListViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            Notifications = new ObservableCollection<INotificationViewModel>();
        }

        public void Handle(Models.Notification message)
        {
            message.Title = string.Concat("Incoming call from ", message.phone_number);
            
            Notifications.Add(new NotificationViewModel { Model = message });
        }
    }
}