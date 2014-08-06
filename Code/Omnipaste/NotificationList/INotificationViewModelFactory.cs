namespace Omnipaste.NotificationList
{
    using Clipboard.Models;
    using Events.Models;
    using Ninject;
    using Omnipaste.Notification;

    public interface INotificationViewModelFactory
    {
        INotificationViewModel Create(Event @event);

        INotificationViewModel Create(Clipping clipping);

        [Inject]
        IKernel Kernel { get; set; }
    }
}