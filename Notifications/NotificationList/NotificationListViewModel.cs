using System.Collections.ObjectModel;
using Caliburn.Micro;
using Notifications.Notification;

namespace Notifications.NotificationList
{
    public class NotificationListViewModel : Conductor<IScreen>.Collection.OneActive, INotificationListViewModel, IHandle<Models.Notification>
    {
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<INotificationViewModel> Notifications { get; set; }

        public NotificationListViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Notifications = new ObservableCollection<INotificationViewModel>();
            Handle(new Models.Notification { phone_number = "0747074372" });
            Handle(new Models.Notification { phone_number = "0747074372" });
        }

        public void Handle(Models.Notification message)
        {
            message.Title = string.Concat("Incoming call from ", message.phone_number);
            
            Notifications.Add(new NotificationViewModel { Model = message });
        }
    }
}