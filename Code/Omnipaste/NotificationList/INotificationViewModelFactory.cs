namespace Omnipaste.NotificationList
{
    using Events.Models;
    using Ninject;
    using Omnipaste.Models;
    using Omnipaste.Notification;

    public interface INotificationViewModelFactory
    {
        INotificationViewModel Create(Event @event);

        INotificationViewModel Create(ClippingModel clipping);

        [Inject]
        IKernel Kernel { get; set; }
    }
}