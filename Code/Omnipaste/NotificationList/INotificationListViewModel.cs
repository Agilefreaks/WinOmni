namespace Omnipaste.NotificationList
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using Omnipaste.EventAggregatorMessages;
    using Omnipaste.Notification;

    public interface INotificationListViewModel : IScreen, IHandle<DismissNotification>
    {
        ObservableCollection<INotificationViewModel> Notifications { get; set; }

        double Height { get; set; }

        void Show();
    }
}